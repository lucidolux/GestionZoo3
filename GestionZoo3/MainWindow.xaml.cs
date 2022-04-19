using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GestionZoo3
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection sqlConnection;
        public MainWindow()
        {
            InitializeComponent();

            string connectionString = ConfigurationManager.ConnectionStrings["GestionZoo3.Properties.Settings.GestionZoo3ConnectionString"].ConnectionString;

            sqlConnection = new SqlConnection(connectionString);

            MuestraZoos();
            MuestraAnimales();
        }


        #region Metodos
        /// <summary>
        /// Metodo que  muestra todos los zoologicos  
        /// </summary>
        private void MuestraZoos()
        {
            try
            {
                string consuslta = "select * from Zoo";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(consuslta, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable zooTabla = new DataTable(); // permite almacenar datos de tablas dentro de un objeto
                    sqlDataAdapter.Fill(zooTabla);

                    ListaZoos.DisplayMemberPath = "Ubicacion"; // nos muestre  todo lo que hay en la columna ubicacion
                    ListaZoos.SelectedValuePath = "Id"; //   dato duro que sirve para identificar  por id
                    ListaZoos.ItemsSource = zooTabla.DefaultView; // muiestra  los valores por defecto

                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());

            }
        }

        private void MuestraAnimales()
        {
            try
            {
                string consuslta = "select * from Animal";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(consuslta, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable animalTable = new DataTable(); // permite almacenar datos de tablas dentro de un objeto
                    sqlDataAdapter.Fill(animalTable);

                    ListaAnimales.DisplayMemberPath = "Nombre"; // nos muestre  todo lo que hay en la columna ubicacion
                    ListaAnimales.SelectedValuePath = "Id"; //   dato duro que sirve para identificar  por id
                    ListaAnimales.ItemsSource = animalTable.DefaultView; // muiestra  los valores por defecto

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());

            }
        }


        /// <summary>
        /// Muestra los  animales actuales en los zoologicos seleccionados
        /// </summary>
        private void MuestraAnimalesAsociados()
        {
            try
            {
                string consuslta = "select * from Animal a Inner Join AnimalZoo az on a.Id = az.AnimalId where az.ZooId = @ZooId";

                SqlCommand sqlCommand = new SqlCommand(consuslta, sqlConnection);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@ZooId", ListaZoos.SelectedValue);

                    DataTable AnimalTabla = new DataTable("@ZooId"); // permite almacenar datos de tablas dentro de un objeto
                    sqlDataAdapter.Fill(AnimalTabla);

                    ListaAnimalesAsociados.DisplayMemberPath = "Nombre"; // nos muestre  todo lo que hay en la columna ubicacion
                    ListaAnimalesAsociados.SelectedValuePath = "Id"; //   dato duro que sirve para identificar  por id
                    ListaAnimalesAsociados.ItemsSource = AnimalTabla.DefaultView; // muiestra  los valores por defecto

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());

            }

        }

        /// <summary>
        ///  Devuelve en el textbox el nombre del zoologico escogido.
        /// </summary>
        private void MuestraZoosElegidosDelTextBx()
        {
            try
            {
                string consuslta = "select Ubicacion from Zoo where Id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(consuslta, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@ZooId", ListaZoos.SelectedValue);

                    DataTable zooDataTable = new DataTable(); // permite almacenar datos de una lista  de tabla dentro de un objeto
                    sqlDataAdapter.Fill(zooDataTable);

                    mitextBox.Text = zooDataTable.Rows[0]["Ubicacion"].ToString();
                    //asigno a mi txtbx  la informacion que este en zooDataTable  en su fila 0 en la columna ubicacion ese valor quiero como un string 
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());

            }
        }

        /// <summary>
        /// Muestra los animales elegidos de la lista en el txtBox
        /// </summary>
        private void MuestraAnimalesElegidosTxtBx()
        {
            try
            {
                string consuslta = "select Nombre from Animal where Id = @AnimalId";
                SqlCommand sqlCommand = new SqlCommand(consuslta, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@AnimalId", ListaAnimales.SelectedValue);

                    DataTable animalDataTable = new DataTable(); // permite almacenar datos de una lista  de tabla dentro de un objeto
                    sqlDataAdapter.Fill(animalDataTable);

                    mitextBox.Text = animalDataTable.Rows[0]["Nombre"].ToString();
                    //asigno a mi txtbx  la informacion que este en zooDataTable  en su fila 0 en la columna ubicacion ese valor quiero como un string 
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());

            }
        }

        private void ListaZoos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MuestraAnimalesAsociados();
            MuestraZoosElegidosDelTextBx();
        }

        private void ListaAnimales_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MuestraAnimalesElegidosTxtBx();
        }

        private void EliminarZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string consulta = "delete from zoo where Id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", ListaZoos.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                MuestraZoos();
            }
        }

        // TODO: CONTINUAR CON EL PROYECTO

        private void AgregarZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string consulta = "insert into zoo values (@Ubicacion)";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Ubicacion", mitextBox.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                MuestraZoos();
            }

        }


        private void AgregarAnimalAZoo_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string consulta = "insert into AnimalZoo values (@ZooID, @AnimalId)";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@AnimalId", ListaAnimales.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@ZooId", ListaZoos.SelectedValue);

                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                MuestraAnimalesAsociados();

            }
        }
        // todo: Agregar metodo eliminar y agregar a la lista de  animalesz zoo.

        private void ActualizarZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string consulta = "Update Zoo set Ubicacion =@Ubicacion  where Id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooID", ListaZoos.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@Ubicacion", mitextBox.Text);

                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
              //  MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                MuestraZoos();
            }
        }

        private void ActualizarAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                string consulta = "Update Animal set Nombre =@Nombre where Id = @Id";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Id", ListaAnimales.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@Nombre", mitextBox.Text);

                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                  MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                MuestraAnimales();
            }
        }
        private void AgregarAnimalALista_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string consulta = "insert into Animal values (@Nombre)";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Nombre", mitextBox.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                MuestraAnimales();

            }

        }

        private void EliminarAnimalLista_Click(object sender, RoutedEventArgs e)
        {


            try
            {
                string consulta = "delete from Animal where Id=@Id";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Id", ListaAnimales.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }
            finally
            {
                sqlConnection.Close();
                MuestraAnimales();
            }
        }


        // todo: IMPLEMENTAR CODIGO BOTON QUITAR  ANIMAL ZOOLOGICO

        private void QuitarAnimalZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string consulta = "delete from AnimalZoo where Id =@AnimalId";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@AnimalId" , ListaAnimalesAsociados.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                MuestraAnimalesAsociados();
            }
        }



        #endregion


    }
}
