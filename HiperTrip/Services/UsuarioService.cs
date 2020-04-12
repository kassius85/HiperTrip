using AutoMapper;
using Entities.DTOs;
using Entities.Helpers;
using Entities.Models;
using HiperTrip.Extensions;
using HiperTrip.Interfaces;
using HiperTrip.Models;
using HiperTrip.Settings;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HiperTrip.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly AppSettings _appSettings;
        private readonly DbHiperTripContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IResultService _resultService;
        private readonly EmailSettings _emailConfiguration;
        private readonly IActionContextAccessor _accessor;
        private readonly IParamGenUsuService _paramGenUsuService;
        private readonly ICambioRestringidoService _cambioRestringidoService;

        private HttpStatusCode httpStatusCode = default;
        private bool resultado = false;
        private string mensaje = string.Empty;

        public UsuarioService(IOptions<AppSettings> appSettings,
                              DbHiperTripContext dbContext,
                              IMapper mapper,
                              IEmailService emailService,
                              IResultService resultService,
                              IOptions<EmailSettings> emailConfiguration,
                              IActionContextAccessor accessor,
                              IParamGenUsuService paramGenUsuService,
                              ICambioRestringidoService cambioRestringidoService)
        {
            if (appSettings.IsNull())
            {
                throw new Exception("No están definidos los parámetros en el archivo de configuración del sistema");
            }

            _appSettings = appSettings.Value;
            _dbContext = dbContext;
            _mapper = mapper;
            _emailService = emailService;
            _resultService = resultService;

            if (!emailConfiguration.IsNull())
            {
                _emailConfiguration = emailConfiguration.Value;
            }

            _accessor = accessor;
            _paramGenUsuService = paramGenUsuService;
            _cambioRestringidoService = cambioRestringidoService;
        }

        public async Task<Dictionary<string, object>> CrearUsuario(UsuarioDto usuarioNuevo)
        {
            httpStatusCode = HttpStatusCode.OK;
            resultado = true;
            mensaje = "";
            Usuario usuario = null;

            if (!usuarioNuevo.IsNull())
            {
                if (await ValidaNuevo(usuarioNuevo).ConfigureAwait(true))
                {
                    if (usuarioNuevo.Contrasena.BuildHashString(out byte[] contrhash, out byte[] contrsalt, out mensaje))
                    {
                        // Mapear datos de DTO al modelo.
                        usuario = _mapper.Map<Usuario>(usuarioNuevo);

                        // Obtener fecha de registro.
                        usuario.FechaRegist = DateTime.Now;

                        // Inicializar la contraseña con Hash y el Salt.
                        usuario.ContrasHash = contrhash;
                        usuario.ContrasSalt = contrsalt;

                        // Generar nuevo código de opción.
                        string maxcod = await _dbContext.Usuario.MaxAsync(x => x.CodUsuario).ConfigureAwait(true) ?? "0";
                        string newcod = (int.Parse(maxcod, CultureInfo.InvariantCulture) + 1).ToString(CultureInfo.InvariantCulture).Trim().PadLeft(15, '0');

                        usuario.CodUsuario = newcod;

                        // Buscar en parámetros generales el código de tipo de cambio que corresponde a activación de cuenta.
                        ParamGenUsu paramGenUsu = await _paramGenUsuService.GetParamGenUsu().ConfigureAwait(true);

                        // Incicializar datos para la solicitud de activación de cuenta.
                        CambioRestringido cambioRestringido = CreaCambioRestringido(usuario, paramGenUsu.CodActiCuenta, out string randomCode);

                        // Salvar datos de usuario.
                        await _dbContext.Usuario.AddAsync(usuario);

                        if (await _dbContext.SaveChangesAsync().ConfigureAwait(true) > 0)
                        {
                            // Enviar correo para activar cuenta.
                            EnviarCorreo(usuario, randomCode, 1);

                            mensaje = "Usuario creado con éxito.";
                        }
                        else
                        {
                            usuario = null;
                            httpStatusCode = HttpStatusCode.BadRequest;
                            resultado = false;
                            mensaje = "Inconsistencia al salvar usuario.";
                        }
                    }
                    else
                    {
                        usuario = null;
                        httpStatusCode = HttpStatusCode.BadRequest;
                        resultado = false;
                    }
                }
            }
            else
            {
                usuario = null;
                httpStatusCode = HttpStatusCode.BadRequest;
                resultado = false;
                mensaje = "Datos de usuario incorrectos.";
            }

            _resultService.AddValue("StatusCode", httpStatusCode);
            _resultService.AddValue(resultado, mensaje);

            if (resultado)
            {
                usuarioNuevo = _mapper.Map<UsuarioDto>(usuario);
                _resultService.AddValue("usuario", usuarioNuevo);
            }

            return _resultService.GetProperties();
        }

        public async Task<Dictionary<string, object>> ActivarCuenta(ActivaCuentaDto activarCuenta)
        {
            httpStatusCode = HttpStatusCode.BadRequest;
            resultado = false;
            mensaje = string.Empty;

            if (!activarCuenta.IsNull())
            {
                // Buscar en parámetros generales el código de tipo de cambio que corresponde a activación de cuenta.
                ParamGenUsu paramGenUsu = await _paramGenUsuService.GetParamGenUsu().ConfigureAwait(true);

                CambioRestringido cambioRestringido = await _cambioRestringidoService.GetUltimoCambioCuenta(activarCuenta.CodUsuario, paramGenUsu.CodActiCuenta);

                if (!cambioRestringido.IsNull())
                {
                    Usuario usuario = cambioRestringido.CodUsuarioNavigation;

                    if (!usuario.IsNull())
                    {
                        if (!usuario.UsuarActivo.IsStringTrue())
                        {
                            // Crear nuevo intento de cambio.
                            IntentoCambio intentoCambio = CreaIntentoCambio(cambioRestringido);

                            // Verificar el código de activación.
                            if (activarCuenta.CodActivacion.VerifyHashCode(usuario.ContrasSalt, usuario.CambioRestringido.FirstOrDefault().CodActivHash))
                            {
                                usuario.UsuarActivo = "S";                                

                                cambioRestringido.IntentoCambio.Add(intentoCambio);

                                if (await _cambioRestringidoService.ModificaUltimoActivaCuenta(cambioRestringido, intentoCambio))
                                {
                                    httpStatusCode = HttpStatusCode.OK;
                                    resultado = true;
                                    mensaje = "Cuenta activada con éxito!";
                                }
                                else
                                {
                                    httpStatusCode = HttpStatusCode.InternalServerError;
                                    mensaje = "Inconsistencia actualizando datos de activación de cuenta.";
                                }
                            }
                            else
                            {
                                intentoCambio.IntenExitoso = "N";
                                int cantIntentos = cambioRestringido.IntentoCambio.Count + 1;

                                cambioRestringido.IntentoCambio.Add(intentoCambio);

                                if (await _cambioRestringidoService.ModificaUltimoActivaCuenta(cambioRestringido, intentoCambio))
                                {
                                    if (cantIntentos < paramGenUsu.CantIntentAct) // Otro intento fallido.
                                    {
                                        mensaje = "El código no es válido.";
                                    }
                                    else // Llegó al límite de intentos de activación de la cuenta.
                                    {
                                        // Incicializar datos para la solicitud de activación de cuenta.
                                        CambioRestringido cambioRestringidoNuevo = CreaCambioRestringido(usuario, paramGenUsu.CodActiCuenta, out string randomCode);

                                        if (await _cambioRestringidoService.InsertaNuevoActivaCuenta(cambioRestringidoNuevo))
                                        {
                                            // Enviar correo para activar cuenta.
                                            EnviarCorreo(usuario, randomCode, 2);

                                            mensaje = "Ha excedido el número máximo de intentos de activación. Favor revisar su correo e intentar con el nuevo código.";
                                        }
                                        else
                                        {
                                            httpStatusCode = HttpStatusCode.InternalServerError;
                                            mensaje = "Inconsistencia salvando datos de activación de cuenta.";
                                        }
                                    }
                                }
                                else
                                {
                                    httpStatusCode = HttpStatusCode.InternalServerError;
                                    mensaje = "Inconsistencia actualizando datos de activación de cuenta.";
                                }                                
                            }
                        }
                        else
                        {
                            mensaje = "La cuenta fue activada con anterioridad.";
                        }
                    }
                    else
                    {
                        httpStatusCode = HttpStatusCode.NotFound;
                        mensaje = "El usuario no existe.";
                    }
                }
                else
                {
                    httpStatusCode = HttpStatusCode.NotFound;
                    mensaje = "No existe solicitud de activación de cuenta pendiente.";
                }
            }
            else
            {
                mensaje = "Favor suministrar datos de activación de la cuenta.";
            }

            _resultService.AddValue("StatusCode", httpStatusCode);
            _resultService.AddValue(resultado, mensaje);

            return _resultService.GetProperties();
        }

        public async Task<Dictionary<string, object>> Autenticarse(UsuarioDto usuarioDto)
        {
            httpStatusCode = HttpStatusCode.OK;
            resultado = true;
            mensaje = "";

            Usuario usuario = null;
            string token = string.Empty;

            if (!usuarioDto.IsNull())
            {
                // Verificar si existe el correo o número de celular
                if (!string.IsNullOrEmpty(usuarioDto.CorreoUsuar))
                {
                    usuario = await _dbContext.Usuario.SingleOrDefaultAsync(x => x.CorreoUsuar.ToUpper() == usuarioDto.CorreoUsuar.ToUpper()).ConfigureAwait(true);
                }
                else
                {
                    if (!string.IsNullOrEmpty(usuarioDto.NumCelular))
                    {
                        usuario = await _dbContext.Usuario.SingleOrDefaultAsync(x => x.NumCelular == usuarioDto.NumCelular).ConfigureAwait(true);
                    }
                    else
                    {
                        httpStatusCode = HttpStatusCode.BadRequest;
                        resultado = false;
                        mensaje = "Debe suministrar el correo o teléfono para la autenticación.";
                    }
                }
            }

            // Verificar si se trajeron datos del usuario.
            if (resultado && !usuario.IsNull())
            {
                string contrasena = usuarioDto.Contrasena ?? string.Empty;
                
                // Verificar que la contraseña es válida.
                if (contrasena.VerifyHashCode(usuario.ContrasSalt, usuario.ContrasHash))
                {
                    // Mapear datos de modelo al DTO.
                    usuarioDto = _mapper.Map<UsuarioDto>(usuario);

                    token = usuarioDto.CodUsuario.BuildToken(usuarioDto.CorreoUsuar, _appSettings.JwtSecretKey, _appSettings.JwtExpireMinutes);
                
                    if (!usuarioDto.UsuarActivo.IsStringTrue())
                    {
                        // Buscar en parámetros generales el código de tipo de cambio que corresponde a activación de cuenta.
                        ParamGenUsu paramGenUsu = await _paramGenUsuService.GetParamGenUsu().ConfigureAwait(true);

                        // Incicializar datos para la solicitud de activación de cuenta.
                        CambioRestringido cambioRestringidoNuevo = CreaCambioRestringido(usuario, paramGenUsu.CodActiCuenta, out string randomCode);

                        if (await _cambioRestringidoService.InsertaNuevoActivaCuenta(cambioRestringidoNuevo))
                        {
                            // Enviar correo para activar cuenta.
                            EnviarCorreo(usuario, randomCode, 2);

                            httpStatusCode = HttpStatusCode.PreconditionRequired; // 428 - alert
                            resultado = false;
                            mensaje = "Su cuenta no ha sido activada. Favor revisar su correo e intentar con el nuevo código.";
                        }
                        else
                        {
                            httpStatusCode = HttpStatusCode.InternalServerError;
                            resultado = false;
                            mensaje = "Inconsistencia salvando datos de activación de cuenta.";
                        }
                    }
                }
                else
                {
                    httpStatusCode = HttpStatusCode.BadRequest;
                    resultado = false;
                    mensaje = "Usuario o contraseña incorrecto.";
                }
            }
            else
            {
                if (resultado)
                {
                    httpStatusCode = HttpStatusCode.NotFound;
                    resultado = false;
                    mensaje = "El usuario no existe.";
                }

                usuarioDto = null;
            }

            _resultService.AddValue("StatusCode", httpStatusCode);
            _resultService.AddValue(resultado, mensaje);

            if (resultado)
            {
                _resultService.AddValue("usuario", usuarioDto);
                _resultService.AddValue("token", token);
            }

            return _resultService.GetProperties();
        }

        public async Task<Dictionary<string, object>> RecuperarCuenta(UsuarioDto usuarioDto)
        {
            httpStatusCode = HttpStatusCode.BadRequest;
            resultado = false;
            mensaje = string.Empty;

            if (!usuarioDto.IsNull())
            {
                if (_emailService.ValidEmail(usuarioDto.CorreoUsuar, _emailConfiguration.RegExp))
                {
                    Usuario usuario = await _dbContext.Usuario.SingleOrDefaultAsync(x => x.CorreoUsuar.ToUpper() == usuarioDto.CorreoUsuar.ToUpper()).ConfigureAwait(true);

                    if (!usuario.IsNull())
                    {
                        // Buscar en parámetros generales el código de tipo de cambio que corresponde a activación de cuenta.
                        ParamGenUsu paramGenUsu = await _paramGenUsuService.GetParamGenUsu().ConfigureAwait(true);

                        if (usuario.UsuarActivo.IsStringTrue())
                        {
                            // Incicializar datos para la solicitud de recuperación de cuenta.
                            CambioRestringido cambioRestringidoNuevo = CreaCambioRestringido(usuario, paramGenUsu.CodRecupCuenta, out string randomCode);

                            // Añadir nueva solicitud de cambio.
                            await _dbContext.CambioRestringido.AddAsync(cambioRestringidoNuevo);

                            // Actualizar datos de usuario.
                            _dbContext.Entry(usuario).State = EntityState.Modified;

                            if (await _dbContext.SaveChangesAsync().ConfigureAwait(true) > 0)
                            {
                                EnviarCorreo(usuario, randomCode, 3);

                                httpStatusCode = HttpStatusCode.OK;
                                resultado = true;
                                mensaje = "Código de recuparación enviado. Favor revise su correo.";
                            }
                            else
                            {
                                httpStatusCode = HttpStatusCode.InternalServerError;
                                mensaje = "Inconsistencia al salvar datos de usuario.";
                            }
                        }
                        else // Lógica para enviar correo de activar cuenta.
                        {
                            // Incicializar datos para la solicitud de activación de cuenta.
                            CambioRestringido cambioRestringidoNuevo = CreaCambioRestringido(usuario, paramGenUsu.CodActiCuenta, out string randomCode);

                            // Añadir nueva solicitud de cambio.
                            await _dbContext.CambioRestringido.AddAsync(cambioRestringidoNuevo);

                            // Actualizar datos de usuario.
                            _dbContext.Entry(usuario).State = EntityState.Modified;

                            if (await _dbContext.SaveChangesAsync().ConfigureAwait(true) > 0)
                            {
                                // Enviar correo para activar cuenta.
                                EnviarCorreo(usuario, randomCode, 2);

                                httpStatusCode = HttpStatusCode.PreconditionRequired; // 428 - alert
                                mensaje = "Debe activar la cuenta antes de recuperar la contraseña. Revise su correo para obtener código.";
                            }
                            else
                            {
                                httpStatusCode = HttpStatusCode.InternalServerError;
                                mensaje = "Inconsistencia al salvar datos de usuario.";
                            }                            
                        }
                    }
                    else
                    {
                        mensaje = "La dirección de correo no está registrada.";
                    }
                }
                else
                {
                    mensaje = "La dirección de correo no es válida.";
                }
            }
            else
            {
                mensaje = "Debe suministrar los datos para la recuperación de la cuenta.";
            }

            _resultService.AddValue("StatusCode", httpStatusCode);
            _resultService.AddValue(resultado, mensaje);

            return _resultService.GetProperties();
        }

        public async Task<Dictionary<string, object>> CambiarContrasena(RecuperaContrasenaDto recuperaContrasena)
        {
            httpStatusCode = HttpStatusCode.BadRequest;
            resultado = false;
            mensaje = string.Empty;

            if (!recuperaContrasena.IsNull())
            {
                // Buscar en parámetros generales el código de tipo de cambio que corresponde a activación de cuenta.
                ParamGenUsu paramGenUsu = await _paramGenUsuService.GetParamGenUsu().ConfigureAwait(true);

                CambioRestringido cambioRestringido = await _cambioRestringidoService.GetUltimoCambioCuenta(recuperaContrasena.CodUsuario, paramGenUsu.CodRecupCuenta);

                if (!cambioRestringido.IsNull())
                {
                    if (!cambioRestringido.IntentoCambio.Any(x => x.IntenExitoso.IsStringTrue()))
                    {
                        Usuario usuario = cambioRestringido.CodUsuarioNavigation;

                        if (!usuario.IsNull())
                        {
                            if (usuario.UsuarActivo.IsStringTrue())
                            {
                                // Crear nuevo intento de cambio.
                                IntentoCambio intentoCambio = CreaIntentoCambio(cambioRestringido);

                                // Verificar el código de recuperación.
                                if (recuperaContrasena.CodRecuperacion.VerifyHashCode(usuario.ContrasSalt, usuario.CambioRestringido.FirstOrDefault().CodActivHash))
                                {
                                    if (recuperaContrasena.Contrasena.BuildHashString(out byte[] contrhash, out byte[] contrsalt, out mensaje))
                                    {
                                        // Inicializar la contraseña con Hash y el Salt.
                                        usuario.ContrasHash = contrhash;
                                        usuario.ContrasSalt = contrsalt;                                    

                                        cambioRestringido.IntentoCambio.Add(intentoCambio);

                                        if (await _cambioRestringidoService.ModificaUltimoActivaCuenta(cambioRestringido, intentoCambio))
                                        {
                                            httpStatusCode = HttpStatusCode.OK;
                                            resultado = true;
                                            mensaje = "Cuenta recuperada con éxito!";
                                        }
                                        else
                                        {
                                            httpStatusCode = HttpStatusCode.InternalServerError;
                                            mensaje = "Inconsistencia actualizando datos de recuperación de contraseña.";
                                        }
                                    }
                                }
                                else
                                {
                                    intentoCambio.IntenExitoso = "N";
                                    int cantIntentos = cambioRestringido.IntentoCambio.Count + 1;

                                    cambioRestringido.IntentoCambio.Add(intentoCambio);

                                    if (await _cambioRestringidoService.ModificaUltimoActivaCuenta(cambioRestringido, intentoCambio))
                                    {
                                        if (cantIntentos < paramGenUsu.CantIntentRecu) // Otro intento fallido.
                                        {
                                            mensaje = "El código de recuperación de contraseña no es válido.";
                                        }
                                        else // Llegó al límite de intentos de activación de la cuenta.
                                        {
                                            // Incicializar datos para la solicitud de recuperación de contraseña.
                                            CambioRestringido cambioRestringidoNuevo = CreaCambioRestringido(usuario, paramGenUsu.CodRecupCuenta, out string randomCode);

                                            if (await _cambioRestringidoService.InsertaNuevoActivaCuenta(cambioRestringidoNuevo))
                                            {
                                                // Enviar correo para recuperar contraseña.
                                                EnviarCorreo(usuario, randomCode, 3);

                                                mensaje = "Ha excedido el número máximo de intentos de recuperación de contraseña. Favor revisar su correo e intentar con el nuevo código.";
                                            }
                                            else
                                            {
                                                httpStatusCode = HttpStatusCode.InternalServerError;
                                                mensaje = "Inconsistencia salvando datos de recuperación de contraseña.";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        httpStatusCode = HttpStatusCode.InternalServerError;
                                        mensaje = "Inconsistencia actualizando datos de recuperación de contraseña.";
                                    }
                                }
                            }
                            else // Lógica para enviar correo de activar cuenta.
                            {
                                // Incicializar datos para la solicitud de activación de cuenta.
                                CambioRestringido cambioRestringidoNuevo = CreaCambioRestringido(usuario, paramGenUsu.CodActiCuenta, out string randomCode);

                                if (await _cambioRestringidoService.InsertaNuevoActivaCuenta(cambioRestringidoNuevo))
                                {
                                    // Enviar correo para activar cuenta.
                                    EnviarCorreo(usuario, randomCode, 2);

                                    httpStatusCode = HttpStatusCode.PreconditionRequired; // 428 - alert
                                    mensaje = "Debe activar la cuenta antes de recuperar la contraseña. Revise su correo para obtener código.";
                                }
                                else
                                {
                                    httpStatusCode = HttpStatusCode.InternalServerError;
                                    mensaje = "Inconsistencia salvando datos de recuperación de contraseña.";
                                }
                            }
                        }
                        else
                        {
                            httpStatusCode = HttpStatusCode.NotFound;
                            mensaje = "El usuario no existe.";
                        }
                    }
                    else
                    {
                        mensaje = "La contraseña fue recuperada con anterioridad. Si olvidó su nueva contraseña debe repetir el proceso de recuperación de contraseña desde el inicio.";
                    }                    
                }
                else
                {
                    httpStatusCode = HttpStatusCode.NotFound;
                    mensaje = "No existe solicitud de recuperación de cuenta pendiente.";
                }
            }
            else
            {
                mensaje = "Debe suministrar los datos para cambiar la contraseña.";
            }

            _resultService.AddValue("StatusCode", httpStatusCode);
            _resultService.AddValue(resultado, mensaje);

            return _resultService.GetProperties();
        }

        public async Task<Dictionary<string, object>> GetUsuarios()
        {
            IList<Usuario> resultado = await _dbContext.Usuario.ToListAsync().ConfigureAwait(true);

            IList<Usuario> listaUsuarios = _mapper.Map<IList<Usuario>>(resultado);

            _resultService.AddValue("listaUsuarios", listaUsuarios);

            return _resultService.GetProperties();
        }

        public async Task<Dictionary<string, object>> GetUsuarioPorId(string id)
        {
            httpStatusCode = HttpStatusCode.BadRequest;
            resultado = false;
            mensaje = string.Empty;

            if (!string.IsNullOrEmpty(id))
            {
                Usuario usuario = await _dbContext.Usuario.SingleOrDefaultAsync(x => x.CodUsuario == id).ConfigureAwait(true);

                if (!usuario.IsNull())
                {
                    UsuarioDto usuarioDto = _mapper.Map<UsuarioDto>(usuario);

                    httpStatusCode = HttpStatusCode.OK;
                    resultado = true;
                    _resultService.AddValue("usuario", usuarioDto);
                }
                else
                {
                    httpStatusCode = HttpStatusCode.NotFound;
                    mensaje = "El usuario no existe.";
                }
            }
            else
            {
                mensaje = "Favor sumistrar una identificación válida.";
            }

            _resultService.AddValue("StatusCode", httpStatusCode);
            _resultService.AddValue(resultado, mensaje);

            return _resultService.GetProperties();
        }

        public async Task<bool> ExisteUsuario(string id)
        {
            return await _dbContext.Usuario.AnyAsync(e => e.CodUsuario == id).ConfigureAwait(true);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------
        // Métodos privados
        //-----------------------------------------------------------------------------------------------------------------------------------
        private async Task<bool> ValidaNuevo(UsuarioDto usuario)
        {
            httpStatusCode = HttpStatusCode.OK;
            resultado = true;
            mensaje = "";

            if (_emailService.ValidEmail(usuario.CorreoUsuar, @_emailConfiguration.RegExp))
            {
                if (!await ExisteCorreoUsuario(usuario.CorreoUsuar).ConfigureAwait(true))
                {
                    return true;
                }
                else
                {
                    httpStatusCode = HttpStatusCode.BadRequest;
                    resultado = false;
                    mensaje = "El correo ya está asociado a otra cuenta.";
                }
            }
            else
            {
                httpStatusCode = HttpStatusCode.BadRequest;
                resultado = false;
                mensaje = "La dirección de correo no es válida.";
            }

            return false;
        }

        private async Task<bool> ExisteCorreoUsuario(string correoUsu)
        {
            return await _dbContext.Usuario.AnyAsync(e => e.CorreoUsuar == correoUsu).ConfigureAwait(true);
        }

        private void EnviarCorreo(Usuario usuario, string codigoAct, int opcion = 1)
        {
            EmailMessage emailMessage = new EmailMessage();

            List<EmailAddress> fromAddresses = new List<EmailAddress>();
            EmailAddress emailFROM = new EmailAddress()
            {
                Name = _emailConfiguration.NameToReply,
                Address = _emailConfiguration.SmtpUsername
            };
            fromAddresses.Add(emailFROM);

            List<EmailAddress> toAddresses = new List<EmailAddress>();
            EmailAddress emailTO = new EmailAddress()
            {
                Name = usuario.NombreCompl,
                Address = usuario.CorreoUsuar
            };
            toAddresses.Add(emailTO);

            emailMessage.FromAddresses = fromAddresses;

            emailMessage.ToAddresses = toAddresses;

            switch (opcion)
            {
                case 1:
                    {
                        emailMessage.Subject = "Account Activation - HiperTrip";

                        emailMessage.Content = string.Format(new CultureInfo("es-Cr"), @"<!DOCTYPE html><html><head><title>{3}</title></head><body><table style=""height: 100 %; width: 500px; font-family: sans-serif; ""><tbody><tr><td><div style=""background-color: #009bd4; color: #ffffff; text-align: center; padding-top: 1px; padding-bottom: 1px;""><h1>Verification Notice!</h1><h2>ACTION REQUIRED</h2></div><div style=""padding: 20px 0px 10px 0px; line-height: 180%; font-size: 18px; color: #565a5c; text-align: justify; border-bottom: 1px #bbb solid;""><span style=""font-weight: bold;"">Notice:</span> To ensure you receive our future emails such as maintenance notices and renewal notices, please add us to your contact list.</div><div style=""padding: 25px 0px 0px 0px;""><p style=""font-size: 18px; color: #565a5c;"">Hi {1}:</p><p style=""font-size: 18px; color: #ff6600; font-weight: bold;"">You're one step away from becoming a HiperTrip member.</p></div><div style=""padding: 10px 0px 0px 0px;""><p style=""font-size: 18px; color: #565a5c;"">Below is your account login information:</p><p style=""font-size: 18px; color: #565a5c;"">Username: <span style=""color: #ff6600; font-weight: bold;"">{0}</span></p></div><div style=""padding: 10px 0px 0px 0px;""><p style=""font-size: 18px; color: #565a5c; font-weight: bold;"">Here It's The Code To Activate Your Account: <span style=""color: #ff6600; font-weight: bold;""> {2} </span></p></div><div style=""padding: 20px 0px 10px 0px; line-height: 180%; font-size: 18px; color: #565a5c; text-align: justify;"">If you have questions or concerns, please <a href=""https://www.hipertrip.com/contact/"" style=""color:#ff6600;text-decoration:none"" target=""_blank"">contact us</a>.</div><div style=""padding: 20px 0px 10px 0px; line-height: 180%; font-size: 18px; color: #565a5c; text-align: justify;"">-HiperTrip Team</div><div style=""padding: 0px 0 5px 0; line-height: 180%; font-size: 18px; color: #565a5c; text-align: center; border-bottom: 1px #bbb solid; border-top: 1px #bbb solid; font-weight: bold;""> DO NOT REPLY TO THIS EMAIL </div></td></tr></tbody></table></body></html>", usuario.CorreoUsuar, usuario.NombreCompl, codigoAct, emailMessage.Subject);

                        break;
                    }

                case 2:
                    {
                        emailMessage.Subject = "Account Activation - HiperTrip";

                        emailMessage.Content = string.Format(new CultureInfo("es-Cr"), @"<!DOCTYPE html><html><head><title>{0}</title></head><body><table style=""height: 100 %; width: 500px; font-family: sans-serif; ""><tbody><tr><td><div style=""background-color: #009bd4; color: #ffffff; text-align: center; padding-top: 1px; padding-bottom: 1px;""><h1>Verification Notice!</h1><h2>ACTION REQUIRED</h2></div><div style=""padding: 20px 0px 10px 0px; line-height: 180%; font-size: 18px; color: #565a5c; text-align: justify; border-bottom: 1px #bbb solid;""><span style=""font-weight: bold;"">Notice:</span> To ensure you receive our future emails such as maintenance notices and renewal notices, please add us to your contact list.</div><div style=""padding: 25px 0px 0px 0px;""><p style=""font-size: 18px; color: #565a5c;"">Hi {1}:</p><p style=""font-size: 18px; color: #ff6600; font-weight: bold;"">You need to activate your account to start being a hipertriper.</p></div><div style=""padding: 10px 0px 0px 0px;""><p style=""font-size: 18px; color: #565a5c; font-weight: bold;"">Here It's The Code To Activate Your Account: <span style=""color: #ff6600; font-weight: bold;""> {2} </span></p></div><div style=""padding: 20px 0px 10px 0px; line-height: 180%; font-size: 18px; color: #565a5c; text-align: justify;"">If you have questions or concerns, please <a href=""https://www.hipertrip.com/contact/"" style=""color:#ff6600;text-decoration:none"" target=""_blank"">contact us</a>.</div><div style=""padding: 20px 0px 10px 0px; line-height: 180%; font-size: 18px; color: #565a5c; text-align: justify;"">-HiperTrip Team</div><div style=""padding: 0px 0 5px 0; line-height: 180%; font-size: 18px; color: #565a5c; text-align: center; border-bottom: 1px #bbb solid; border-top: 1px #bbb solid; font-weight: bold;""> DO NOT REPLY TO THIS EMAIL </div></td></tr></tbody></table></body></html>", emailMessage.Subject, usuario.NombreCompl, codigoAct);

                        break;
                    }

                case 3:
                    {
                        emailMessage.Subject = "Password Recovery - HiperTrip";

                        emailMessage.Content = string.Format(new CultureInfo("es-Cr"), @"<!DOCTYPE html><html><head><title>{0}</title></head><body><table style=""height: 100 %; width: 500px; font-family: sans-serif; ""><tbody><tr><td><div style=""background-color: #009bd4; color: #ffffff; text-align: center; padding-top: 1px; padding-bottom: 1px;""><h1>Verification Notice!</h1><h2>ACTION REQUIRED</h2></div><div style=""padding: 20px 0px 10px 0px; line-height: 180%; font-size: 18px; color: #565a5c; text-align: justify; border-bottom: 1px #bbb solid;""><span style=""font-weight: bold;"">Notice:</span> To ensure you receive our future emails such as maintenance notices and renewal notices, please add us to your contact list.</div><div style=""padding: 25px 0px 0px 0px;""><p style=""font-size: 18px; color: #565a5c;"">Hi {1}:</p><p style=""font-size: 18px; color: #ff6600; font-weight: bold;"">We got a request to reset your HiperTrip password.</p></div><div style=""padding: 10px 0px 0px 0px;""><p style=""font-size: 18px; color: #565a5c; font-weight: bold;"">Here It's The Code To Recover Your Password: <span style=""color: #ff6600; font-weight: bold;""> {2} </span></p></div><div style=""padding: 20px 0px 10px 0px; line-height: 180%; font-size: 18px; color: #565a5c; text-align: justify;"">If you ignore this message, your password will not be changed. If you didn't request a password reset, <a href=""https://www.hipertrip.com/contact/"" style=""color:#ff6600;text-decoration:none"" target=""_blank"">let us know</a>.</div><div style=""padding: 20px 0px 10px 0px; line-height: 180%; font-size: 18px; color: #565a5c; text-align: justify;"">-HiperTrip Team</div><div style=""padding: 0px 0 5px 0; line-height: 180%; font-size: 18px; color: #565a5c; text-align: center; border-bottom: 1px #bbb solid; border-top: 1px #bbb solid; font-weight: bold;""> DO NOT REPLY TO THIS EMAIL </div></td></tr></tbody></table></body></html>", emailMessage.Subject, usuario.NombreCompl, codigoAct);

                        break;
                    }
            }

            // Enviar correo para activar cuenta.
            _emailService.SendEmail(emailMessage, _emailConfiguration);
        }

        private CambioRestringido CreaCambioRestringido(Usuario usuario, string codTipCambCuenta, out string randomCode)
        {
            // Generar código de activación con hash.
            int tamano = 6;
            randomCode = tamano.RandomString();

            CambioRestringido cambioRestringido = new CambioRestringido()
            {
                CodUsuario = usuario.CodUsuario,
                FechaSolic = DateTime.Now,
                CodActivHash = randomCode.BuildHashCode(usuario.ContrasSalt),
                IpSolicita = _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString(),
                CodTipCambCuenta = codTipCambCuenta,
                CodUsuarioNavigation = usuario
            };

            usuario.CambioRestringido.Add(cambioRestringido);

            return cambioRestringido;
        }

        private IntentoCambio CreaIntentoCambio(CambioRestringido cambioRestringido)
        {
            // Generar nuevo código de intento.
            string maxcod = cambioRestringido.IntentoCambio.Max(x => x.CodIntento) ?? "0";
            string newcod = (int.Parse(maxcod) + 1).ToString().Trim().PadLeft(2, '0');

            IntentoCambio intentoCambio = new IntentoCambio()
            {
                CodUsuario = cambioRestringido.CodUsuarioNavigation.CodUsuario,
                FechaSolic = cambioRestringido.FechaSolic,
                CodIntento = newcod,
                IpIntento = _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString(),
                FechaIntento = DateTime.Now,
                IntenExitoso = "S",
                CambioRestringido = cambioRestringido
            };

            return intentoCambio;
        }
    }
}