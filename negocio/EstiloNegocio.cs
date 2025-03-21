using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dominio;

namespace negocio
{
	public class EstiloNegocio
	{
		List<Estilo> Lista = new List<Estilo>();
		AccesoDatos datos = new AccesoDatos();
		public List<Estilo> listar()
		{
			try
			{

				datos.setearConsulta("Select Id, Descripcion from ESTILOS");
				datos.ejecutarLectura();

				while (datos.Lector.Read())
				{
					Estilo aux = new Estilo();
					aux.Id = (int)datos.Lector["Id"];
					aux.Descripcion = (string)datos.Lector["Descripcion"];

					Lista.Add(aux);
				}

				return Lista;
			}
			catch (Exception ex)
			{

				throw ex;
			}  
			 finally
			{
				datos.cerrarConexion();

		    }
		}  
    }
}
