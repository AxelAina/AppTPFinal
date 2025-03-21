using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using dominio;
using System.Security.Cryptography.X509Certificates;
using System.CodeDom;
using System.Reflection;
using System.ComponentModel;

namespace negocio
{
    public class DiscoNegocio
    {

        public List<Disco> listar()
        {
            List<Disco> lista = new List<Disco>();
            SqlConnection conexion = new SqlConnection();
            SqlCommand comando = new SqlCommand();
            SqlDataReader lector;

            try
            {
                conexion.ConnectionString = "server=.\\SQLEXPRESS; database=DISCOS_DB; integrated security=true;";
                comando.CommandType = System.Data.CommandType.Text;
                comando.CommandText = "SELECT D.Titulo, D.CantidadCanciones, D.UrlImagenTapa, D.IdEstilo, D.FechaLanzamiento, D.Id, E.Descripcion AS Tipo FROM DISCOS D JOIN ESTILOS E ON E.Id = D.IdTipoEdicion WHERE D.Activo = 1";
                //"SELECT Titulo, CantidadCanciones, UrlImagenTapa, IdEstilo, FechaLanzamiento, D.Id, E.Descripcion AS Tipo FROM DISCOS D, ESTILOS E WHERE E.Id = D.IdTipoEdicion And D.Activo = 1";
                comando.Connection = conexion;

                conexion.Open();
                lector = comando.ExecuteReader();

                while (lector.Read())
                {

                    Disco aux = new Disco();
                    aux.Id = (int)lector["Id"];
                    aux.Titulo = (string)lector["Titulo"];
                    aux.CantidadCanciones = lector.GetInt32(lector.GetOrdinal("CantidadCanciones"));

                    if (!(lector["UrlImagenTapa"] is DBNull))
                    aux.UrlImagenTapa = (string)lector["UrlImagenTapa"];
                    aux.Tipo = new Estilo();
                    aux.Tipo.Descripcion = (string)lector["Tipo"]; // Ahora debería funcionar
                    aux.FechaLanzamiento = (DateTime) lector["FechaLanzamiento"];



                    lista.Add(aux);
                }

                conexion.Close();
                return lista;
            }
            catch (Exception ex)
            {
                throw ex; // Mantiene el stack trace original
            }
        }
        public void agregar(Disco nuevo)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                //datos.setearConsulta("Insert into DISCOS (Titulo, CantidadCanciones, FechaLanzamiento, UrlImagenTapa) values ('" + nuevo.Titulo + "', '" + nuevo.CantidadCanciones + "' @FechaLanzamiento '" + nuevo.UrlImagenTapa + "')");
                //datos.setearConsulta("INSERT INTO DISCOS (Titulo, CantidadCanciones, FechaLanzamiento, UrlImagenTapa) " +
                //"VALUES ('" + nuevo.Titulo + "', '" + nuevo.CantidadCanciones + "', '" + nuevo.FechaLanzamiento + "', '" + nuevo.UrlImagenTapa + "')");


                //datos.setearConsulta("insert into DISCOS (Titulo, FechaLanzamiento, CantidadCanciones, UrlImagenTapa) values ('" + nuevo.Titulo + "', '" + nuevo.FechaLanzamiento + "', " + nuevo.CantidadCanciones + ", '" + nuevo.UrlImagenTapa + "')");
                //datos.setearParametro("@FechaLanzamiento", nuevo.FechaLanzamiento);
                //datos.setearParametro("@UrlImagenTapa", nuevo.UrlImagenTapa);
                //datos.ejecutarAccion();

                // Definir la consulta con parámetros
                datos.setearConsulta("INSERT INTO DISCOS (Titulo, FechaLanzamiento, CantidadCanciones, UrlImagenTapa) " +
                                      "VALUES (@Titulo, @FechaLanzamiento, @CantidadCanciones, @UrlImagenTapa)");

                // Establecer los parámetros para la consulta
                datos.setearParametro("@Titulo", nuevo.Titulo);
                datos.setearParametro("@FechaLanzamiento", nuevo.FechaLanzamiento);
                datos.setearParametro("@CantidadCanciones", nuevo.CantidadCanciones);
                datos.setearParametro("@UrlImagenTapa", nuevo.UrlImagenTapa);

                // Ejecutar la acción de inserción
                datos.ejecutarAccion();
               
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
   
        public void modificar(Disco modificar)
        {

            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("update DISCOS set Titulo = @Titulo, FechaLanzamiento = @FechaLanzamiento, CantidadCanciones = @CantidadCanciones, UrlImagenTapa = @UrlImagentapa Where Id = @Id");
                datos.setearParametro("@Titulo", modificar.Titulo);
                datos.setearParametro("@FechaLanzamiento", modificar.FechaLanzamiento);
                datos.setearParametro("@CantidadCanciones", modificar.CantidadCanciones);
                datos.setearParametro("@UrlImagenTapa", modificar.UrlImagenTapa);
                datos.setearParametro("@Id", modificar.Id);

                datos.ejecutarAccion();
                

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
        public void eliminar(int id)
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();
                datos.setearConsulta("delete from DISCOS where id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void eliminarLogico (int id)
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();
                datos.setearConsulta("update DISCOS set Activo = 0 where id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();

            }
            catch (Exception ex)
            {       

                throw ex;
            }
        }

        public List<Disco> filtrar(string campo, string criterio, string filtro)
        {
            List<Disco> lista = new List<Disco>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                string consulta = "SELECT Titulo, CantidadCanciones, UrlImagenTapa, IdEstilo, FechaLanzamiento, D.Id, E.Descripcion AS Tipo FROM DISCOS D, ESTILOS E WHERE E.Id = D.IdTipoEdicion And D.Activo = 1 And ";
                if(campo == "CantidadCanciones")
                {
                    switch (criterio)
                    {
                        case "Mayor a":
                            consulta += "CantidadCanciones > " + filtro;
                            break;
                        case "Menor a":
                            consulta += "CantidadCanciones < " + filtro;
                            break;
                        default:
                            consulta += "CantidadCanciones = " + filtro;
                            break;
                    }
                } 
                else if(campo == "Titulo")
                {
                   switch (criterio)
                   {
                        case "Comienza con":
                           consulta += "Titulo like '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += "Titulo like '%" + filtro + "'";
                            break;
                        default:
                            consulta += "Titulo like '%" + filtro + "%'";
                            break;
                   }
                } 
                else
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "E.Descripcion like '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += "E.Descripcion like '%" + filtro + "'";
                            break;
                        default:
                            consulta += "E.Descripcion like '%" + filtro + "%'";
                            break;
                    }
                }               

                   datos.setearConsulta(consulta);
                   datos.ejecutarLectura();
                   while (datos.Lector.Read())
                   {

                    Disco aux = new Disco();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Titulo = (string)datos.Lector["Titulo"];
                    aux.CantidadCanciones = datos.Lector.GetInt32(datos.Lector.GetOrdinal("CantidadCanciones"));

                    if (!(datos.Lector["UrlImagenTapa"] is DBNull))
                        aux.UrlImagenTapa = (string)datos.Lector["UrlImagenTapa"];

                    aux.Tipo = new Estilo();
                    aux.Tipo.Descripcion = (string)datos.Lector["Tipo"]; // Ahora debería funcionar
                    aux.FechaLanzamiento = (DateTime)datos.Lector["FechaLanzamiento"];

                    lista.Add(aux);
                   }

                   return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
