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
using System.Data;//ADO.NET
using sisgesoriadao.Model;
using sisgesoriadao.Implementation;
namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winUsuario.xaml
    /// </summary>
    public partial class winUsuario : Window
    {
        public winUsuario()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtBlockWelcome.Text = Session.NombreUsuario;
            ComboboxItem item = new ComboboxItem();
            item.Text = "ADMINISTRADOR";
            item.Value = 1;
            cbxRol.Items.Add(item);
            ComboboxItem item2 = new ComboboxItem();
            item2.Text = "VENDEDOR";
            item2.Value = 2;
            cbxRol.Items.Add(item2);
            cbxRol.SelectedIndex = 0;
            //retorna el valor de 1:
            //MessageBox.Show((cbxRol.SelectedItem as ComboboxItem).Value.ToString());
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            winMainAdmin winMainAdmin = new winMainAdmin();
            winMainAdmin.Show();
            this.Close();
        }

        private void TextBoxUppercase(object sender, KeyEventArgs e)
        {
            TextBox currentContainer = ((TextBox)sender);
            int caretPosition = currentContainer.SelectionStart;

            currentContainer.Text = currentContainer.Text.ToUpper();
            currentContainer.SelectionStart = caretPosition++;
        }
        public class ComboboxItem
        {
            public string Text { get; set; }
            public byte Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }
    }
}
