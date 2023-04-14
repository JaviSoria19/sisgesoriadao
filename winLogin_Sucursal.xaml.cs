using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using sisgesoriadao.Model;
using sisgesoriadao.Implementation;
using System.Data;

namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winLogin_Sucursal.xaml
    /// </summary>
    public partial class winLogin_Sucursal : Window
    {
        SucursalImpl implSucursal;
        public winLogin_Sucursal()
        {
            InitializeComponent();
        }        
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Session.Sucursal_IdSucursal = (cbxSucursal.SelectedItem as ComboboxItem).Valor;
            implSucursal = new SucursalImpl();
            implSucursal.GetBranchForSession(Session.Sucursal_IdSucursal);
            switch (Session.Rol)
            {
                case 1:
                    winMainAdmin winMainAdmin = new winMainAdmin();
                    winMainAdmin.Show();
                    this.Close();
                break;
                case 2:
                    winMainSeller winMainSeller = new winMainSeller();
                    winMainSeller.Show();
                    this.Close();
                break;
                default:
                    MessageBox.Show("Algo salió mal, intente nuevamente.");
                break;
            }
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            winLogin winLogin = new winLogin();
            winLogin.Show();
            this.Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbxGetSucursalFromDatabase();
        }
        void cbxGetSucursalFromDatabase()
        {
            try
            {
                List<ComboboxItem> listcomboboxSucursal = new List<ComboboxItem>();
                DataTable dataTable = new DataTable();
                implSucursal = new SucursalImpl();
                dataTable = implSucursal.SelectForComboBox();
                listcomboboxSucursal = (from DataRow dr in dataTable.Rows
                                        select new ComboboxItem()
                                        {
                                            Valor = Convert.ToByte(dr["idSucursal"]),
                                            Nombre = dr["nombreSucursal"].ToString()
                                        }).ToList();
                foreach (var item in listcomboboxSucursal)
                {
                    cbxSucursal.Items.Add(item);
                }
                cbxSucursal.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public class ComboboxItem
        {
            public string Nombre { get; set; }
            public byte Valor { get; set; }
            public ComboboxItem()
            {

            }
            public override string ToString()
            {
                return Nombre;
            }
        }
    }
}
