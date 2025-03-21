using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio;
using negocio;
using System.Configuration;
using System.IO;

namespace winform_app
{
    public partial class frmAltaDisco : Form
    {

        private Disco disco = null;
        private OpenFileDialog archivo = null;

        public frmAltaDisco()
        {
            InitializeComponent();
        }
        public frmAltaDisco(Disco disco)
        {
            InitializeComponent();
            this.disco = disco;
            Text = "Modificar Disco";
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {

            //Disco disc = new Disco();
            DiscoNegocio negocio = new DiscoNegocio();

            try
            {

                if(disco == null)
                  disco = new Disco();
                
                disco.Titulo = txtTitulo.Text;
                disco.CantidadCanciones = int.Parse(txtCantidadCanciones.Text);
                disco.Tipo = new Estilo();
                disco.UrlImagenTapa = txtUrlImagen.Text;
                disco.Tipo.Descripcion = txtTipo.Text;
                disco.FechaLanzamiento = DateTime.Now;

                if (disco.Id != 0)
                {
                    negocio.modificar(disco);
                    MessageBox.Show("Modificado exitosamente");
                }
                else
                {
                    negocio.agregar(disco);
                    MessageBox.Show("Agregado exitosamente");
                    Refresh();

                }
                
                // Guardo imagen si la levantó localmente
                if(archivo != null && (txtUrlImagen.Text.ToUpper().Contains("HTTP")))
                {
                  File.Copy(archivo.FileName, ConfigurationManager.AppSettings["disco-app"] + archivo.SafeFileName);
                }

                Close();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void frmAltaDisco_Load(object sender, EventArgs e)
        {
            DiscoNegocio elementoNegocio = new DiscoNegocio();

            try
            {
                dtpFecha.Value = DateTime.Now.Date;  // Por ejemplo, poner la fecha actual como valor inicial

                if (disco != null)
                {
                    txtTitulo.Text = disco.Titulo.ToString();
                    txtTipo.Text = disco.Tipo.ToString();
                 // txtTipo.Text = disco.Tipo.Descripcion;
                    txtUrlImagen.Text = disco.UrlImagenTapa;
                    txtCantidadCanciones.Text = disco.CantidadCanciones.ToString();
                    cargarImagen(disco.UrlImagenTapa);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void txtUrlImagen_Leave(object sender, EventArgs e)
        {
            cargarImagen(txtUrlImagen.Text);
        }


        private void cargarImagen(string imagen)
        {
            try
            {
                ptbDisco.Load(imagen);
            }
            catch (Exception ex)
            {
                ptbDisco.Load("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRQpZaeWxczipxrTdSIThz5hmwrRYhEeeAl5A&s");
            }
        }

        private void btnAgregarImagen_Click(object sender, EventArgs e) //Toco botón
        {
            archivo = new OpenFileDialog(); // Abre el archivo / Crea el objeto
            archivo.Filter = "jpg|*.jpg;|png|*.png"; // Se prepara con el filtro de JPG.
            if (archivo.ShowDialog() == DialogResult.OK) // Abre la ventana y elijo
            {
               txtUrlImagen.Text = archivo.FileName; // Lee el archivo y lo guarda en la caja de texto
               cargarImagen(archivo.FileName); // Muestra la imagen

                // Guardo la imagen. Me copia el archivo en esta carpeta
                //File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-folder"] + archivo.SafeFileName);
            }


           
        }
    }
}
