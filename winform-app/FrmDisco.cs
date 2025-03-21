using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using dominio;
using negocio;

namespace winform_app
{
    public partial class frmDiscos : Form
    {

        private List<Disco> listaDisco;
        public frmDiscos()
        {
            InitializeComponent();
        }

        private void frmDiscos_Load(object sender, EventArgs e)
        {
            cargar();
            cboCampo.Items.Add("Titulo");
            cboCampo.Items.Add("CantidadCanciones");
            cboCampo.Items.Add("Tipo");

        }

        private void dgvDiscos_SelectionChanged(object sender, EventArgs e)
        {

           if (dgvDiscos.CurrentRow != null)
            {

            Disco seleccionado = (Disco) dgvDiscos.CurrentRow.DataBoundItem;
            cargarImagen(seleccionado.UrlImagenTapa);   
                
            }
        }

        private void cargar()
        {
             DiscoNegocio negocio = new DiscoNegocio();

            try
             {
             listaDisco = negocio.listar();
             dgvDiscos.DataSource = listaDisco;
             dgvDiscos.Refresh();
             ocultarColumnas();
             cargarImagen(listaDisco[0].UrlImagenTapa);
             dgvDiscos.Refresh();
            }
              catch (Exception ex)
            {

            MessageBox.Show(ex.ToString());

        }
            
        }

        private void ocultarColumnas()
        {
            dgvDiscos.Columns["UrlImagenTapa"].Visible = false;
            dgvDiscos.Columns["Id"].Visible = false;
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

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAltaDisco alta = new frmAltaDisco();
            alta.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            Disco seleccionado;
            seleccionado = (Disco)dgvDiscos.CurrentRow.DataBoundItem;

            frmAltaDisco modificar = new frmAltaDisco(seleccionado);
            modificar.ShowDialog();
            cargar();
        }

        private void btnEliminarFisico_Click(object sender, EventArgs e)
        {
            eliminar();
        }
        private void btnEliminarLogica_Click(object sender, EventArgs e)
        {
            eliminar(true);
        }


        private void eliminar (bool logico = false)
        {
            DiscoNegocio negocio = new DiscoNegocio();
            Disco seleccionado;

            try
            {
                DialogResult respuesta = MessageBox.Show("¿Está seguro que quiere eliminar?", "Eliminando", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (respuesta == DialogResult.Yes)
                {
                    seleccionado = (Disco)dgvDiscos.CurrentRow.DataBoundItem;

                    if(logico)
                       negocio.eliminarLogico(seleccionado.Id);
                    else 
                       negocio.eliminar(seleccionado.Id);

                       cargar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private bool validarFiltro()
        {
            if (cboCampo.SelectedIndex < 0) 
            {
                MessageBox.Show("Por favor, seleccione el campo para filtrar.");
                return true;
            }
            if (cboCriterio.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, seleccione el campo para filtrar.");
                return true;
            }
            if (cboCampo.SelectedItem.ToString() == "CantidadCanciones")
            {
                 if (string.IsNullOrEmpty(txtFiltroAvanzado.Text))
                 {
                    MessageBox.Show("Debes cargar el filtro númerico...");
                    return true;
                 }
                 if(!(soloNumeros(txtFiltroAvanzado.Text)))
                 {
                    MessageBox.Show("Sólo nros para filtrar por un campo númerico...");
                     return true;
                 }
                    
            }

               return false;
        }


        private bool soloNumeros (string cadena)
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter)))
                    return false;
            }
             
             return true;
        }



        private void btnFiltro_Click(object sender, EventArgs e)
        {
            DiscoNegocio negocio = new DiscoNegocio();

            try
            {

                if (validarFiltro())
                {
                    return;
                }

                string campo = cboCampo.SelectedItem.ToString();
                string criterio = cboCriterio.SelectedItem.ToString();
                string filtro = txtFiltroAvanzado.Text;
                dgvDiscos.DataSource = negocio.filtrar(campo, criterio, filtro);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            List<Disco> listaFiltrada;
            string filtro = txtFiltro.Text;

            if (filtro.Length >= 3)
            {
                listaFiltrada = listaDisco.FindAll(x => x.Titulo.ToUpper().Contains(filtro.ToUpper()) || x.Tipo.Descripcion.ToUpper().Contains(filtro.ToUpper()));
            }
            else
            {
                listaFiltrada = listaDisco;
            }

            dgvDiscos.DataSource = null;
            dgvDiscos.DataSource = listaFiltrada;
            ocultarColumnas();
        }

        private void cboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cboCampo.SelectedItem.ToString();
            if(opcion == "CantidadCanciones")
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Mayor a"); 
                cboCriterio.Items.Add("Menor a");
                cboCriterio.Items.Add("Igual a");
            }
            else
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Comienza con");
                cboCriterio.Items.Add("Termina con");
                cboCriterio.Items.Add("Contiene");
            }
        }
    }
}
