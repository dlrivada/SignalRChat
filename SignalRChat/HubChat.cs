using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using SignalRChat.Modelo;

namespace SignalRChat
{
    public class HubChat : Hub
    {
        public List<Usuario> Usuarios { get; set; }

        public List<Mensaje> Mensajes { get; set; }

        public HubChat()
        {
            Usuarios = new List<Usuario>();
            Mensajes = new List<Mensaje>();
        }

        public void Conectar(string nombre)
        {
            var usuario = new Usuario() { Id = Context.ConnectionId, Nombre = nombre };

            if (!Usuarios.All(o => o.Id != usuario.Id))
                return;

            Usuarios.Add(usuario);

            Clients.Caller.onConnectedo(usuario.Id, nombre, Usuarios, Mensajes);
            Clients.AllExcept(usuario.Id).onNevoConnected(usuario.Id, nombre);
        }

        public void EnviarMansaje(string usuario, string mensaje)
        {
            Mensajes.Add(new Mensaje() { Contenido = mensaje, Usuario = usuario });

            if (Mensajes.Count > 30)
                Mensajes.RemoveAt(0);

            Clients.All.enviadoMensaje(usuario, mensaje);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var item = Usuarios.FirstOrDefault(o => o.Id == Context.ConnectionId);
            if (item == null)
                return base.OnDisconnected(stopCalled);

            Usuarios.Remove(item);
            Clients.All.usuarioDesconectado(item.Id, item.Nombre);

            return base.OnDisconnected(stopCalled);
        }
    }
}