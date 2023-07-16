using sisgesoriadao.Implementation;
using sisgesoriadao.Model;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace sisgesoriadao
{
    /// <summary>
    /// Lógica de interacción para winAjuste.xaml
    /// </summary>
    public partial class winAjuste : Window
    {
        AjusteImpl implAjuste;
        Ajuste ajuste;
        public winAjuste()
        {
            InitializeComponent();
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCambioDolar.Text) != true && string.IsNullOrEmpty(txtLimiteDescuento.Text) != true)
            {
                ajuste.CambioDolar = double.Parse(txtCambioDolar.Text.Trim());
                ajuste.LimiteDescuento = byte.Parse(txtLimiteDescuento.Text.Trim());
                implAjuste = new AjusteImpl();
                try
                {
                    int n = implAjuste.Update(ajuste);
                    if (n > 0)
                    {
                        Session.Ajuste_Cambio_Dolar = ajuste.CambioDolar;
                        Session.Ajuste_Limite_Descuento = ajuste.LimiteDescuento;
                        MessageBox.Show("¡AJUSTES MODIFICADOS EXITOSAMENTE!");
                        this.Close();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Transacción no completada, comuníquese con el Administrador de Sistemas.");
                }
            }
            else
            {
                MessageBox.Show("Por favor rellene los campos obligatorios. (*)");
            }
        }
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetSettings();
        }
        private void txtCambioDolar_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private void txtLimiteDescuento_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed2(e.Text);
        }
        void GetSettings()
        {
            try
            {
                implAjuste = new AjusteImpl();
                ajuste = implAjuste.Get();
                if (ajuste != null)
                {
                    txtCambioDolar.Text = ajuste.CambioDolar.ToString().Trim();
                    txtLimiteDescuento.Text = ajuste.LimiteDescuento.ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }
        //--------->VALIDACIÓN PARA QUE EL TEXTBOX SOLO PERMITA NÚMEROS (Y EN ESTE CASO, UNA COMA.)<---------
        private static readonly Regex _regex = new Regex("[^0-9.]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        //------------------------------------------------------><---------------------------------------------
        //--------->VALIDACIÓN PARA QUE EL TEXTBOX SOLO PERMITA NÚMEROS<---------
        private static readonly Regex _regex2 = new Regex("[^0-9]+"); //regex that matches disallowed text
        private static bool IsTextAllowed2(string text)
        {
            return !_regex2.IsMatch(text);
        }
        //------------------------------------------------------><---------------------------------------------
    }
}
