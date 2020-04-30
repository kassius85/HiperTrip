using Entities.DTOs;
using Entities.Models;
using HiperTrip.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HiperTrip.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // POST: api/Usuario
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PostUsuario(UsuarioDto usuario)
        {
            return new ObjectResult(await _usuarioService.CrearUsuario(usuario).ConfigureAwait(true));
        }

        // POST: api/Usuario/Activate -- Activar cuenta de usuario
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [AllowAnonymous]
        [HttpPost]
        [Route("Activate")]
        public async Task<IActionResult> ActivarCuenta(ActivaCuentaDto activarCuenta)
        {
            return new ObjectResult(await _usuarioService.ActivarCuenta(activarCuenta).ConfigureAwait(true));
        }

        // POST: api/Usuario/Login -- Autenticarse
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Autenticarse(UsuarioDto usuario)
        {
            return new ObjectResult(await _usuarioService.Autenticarse(usuario).ConfigureAwait(true));
        }

        // POST: api/Usuario/Recover -- Recuperar cuenta
        [AllowAnonymous]
        [HttpPost]
        [Route("Recover")]
        public async Task<IActionResult> RecuperarCuenta(UsuarioDto usuario)
        {
            return new ObjectResult(await _usuarioService.RecuperarCuenta(usuario).ConfigureAwait(true));
        }

        // POST: api/Usuario/ChangePassword -- Cambiar contraseña
        [AllowAnonymous]
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> CambiarContrasena(RecuperaContrasenaDto recuperaContrasena)
        {
            return new ObjectResult(await _usuarioService.CambiarContrasena(recuperaContrasena).ConfigureAwait(true));
        }

        // GET: api/Usuario
        [HttpGet]
#pragma warning disable CS1998 // El método asincrónico carece de operadores "await" y se ejecutará de forma sincrónica. Puede usar el operador 'await' para esperar llamadas API que no sean de bloqueo o 'await Task.Run(...)' para hacer tareas enlazadas a la CPU en un subproceso en segundo plano.
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuario()
#pragma warning restore CS1998 // El método asincrónico carece de operadores "await" y se ejecutará de forma sincrónica. Puede usar el operador 'await' para esperar llamadas API que no sean de bloqueo o 'await Task.Run(...)' para hacer tareas enlazadas a la CPU en un subproceso en segundo plano.
        {
            return null; // await _context.Usuario.ToListAsync();
        }

        // GET: api/Usuario/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuario(string id)
        {
            return new ObjectResult(await _usuarioService.GetUsuarioPorId(id).ConfigureAwait(true));
        }

        // PUT: api/Usuario/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
#pragma warning disable CS1998 // El método asincrónico carece de operadores "await" y se ejecutará de forma sincrónica. Puede usar el operador 'await' para esperar llamadas API que no sean de bloqueo o 'await Task.Run(...)' para hacer tareas enlazadas a la CPU en un subproceso en segundo plano.
        public async Task<IActionResult> PutUsuario(string id, Usuario usuario)
#pragma warning restore CS1998 // El método asincrónico carece de operadores "await" y se ejecutará de forma sincrónica. Puede usar el operador 'await' para esperar llamadas API que no sean de bloqueo o 'await Task.Run(...)' para hacer tareas enlazadas a la CPU en un subproceso en segundo plano.
        {
            if (id != usuario.CodUsuario)
            {
                return BadRequest();
            }

            /*_context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _usuarioService.ExisteUsuario(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }*/

            return NoContent();
        }

        // DELETE: api/Usuario/5
        [HttpDelete("{id}")]
#pragma warning disable CS1998 // El método asincrónico carece de operadores "await" y se ejecutará de forma sincrónica. Puede usar el operador 'await' para esperar llamadas API que no sean de bloqueo o 'await Task.Run(...)' para hacer tareas enlazadas a la CPU en un subproceso en segundo plano.
        public async Task<ActionResult<Usuario>> DeleteUsuario(string id)
#pragma warning restore CS1998 // El método asincrónico carece de operadores "await" y se ejecutará de forma sincrónica. Puede usar el operador 'await' para esperar llamadas API que no sean de bloqueo o 'await Task.Run(...)' para hacer tareas enlazadas a la CPU en un subproceso en segundo plano.
        {
            Usuario usuario = default; //await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            //_context.Usuario.Remove(usuario);
            //await _context.SaveChangesAsync();

            return usuario;
        }
    }
}