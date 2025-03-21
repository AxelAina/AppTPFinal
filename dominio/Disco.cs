using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dominio
{
     public class Disco
    {

        public int Id { get; set; }

        [DisplayName ("Título")]
        public string Titulo {  get; set; }

        [DisplayName("Cantidad de Canciones")]
        public int CantidadCanciones { get; set; }
        public string UrlImagenTapa {  get; set; }  
        public Estilo Tipo { get; set; }

        [DisplayName("Fecha de Lanzamiento")]
        public DateTime FechaLanzamiento { get; set; }
    }
}
