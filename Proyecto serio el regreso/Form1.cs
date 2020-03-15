﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//Esto es un comentario creado desde mi celular

namespace Proyecto_serio_el_regreso
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            tipos_dato = new string[]{"Numerico", "Nominal", "Binario Asimetrico", "Binario Simetrico", "Texto"};
            cant_instancias = 0;
            cant_columnas = 0;
            datoTexto = "";
        }

        private int cant_instancias;
        private int cant_columnas;
        private string datoTexto;
        private string []tipos_dato;
        private Dictionary<string, int> valores_faltantes;
        private Dictionary<string, KeyValuePair<string, string>> encabezado;
        private Dictionary<string, List<string>> instancias;
        private OpenFileDialog archivo;
        //direccion momentanea del archivo properties
        private OpenFileDialog properties;
        private KeyValuePair<string,string> tipoTexto = new KeyValuePair<string, string>("Texto", "/b(?<word>)/b");

        private Dictionary<string, int> valoresFaltantes()
        {
            Dictionary<string, int> lista = new Dictionary<string, int>();

            foreach(string columna in encabezado.Keys)
            {

                lista[columna] = instancias[columna].FindAll(x => x.Equals("?")).Count();
            }

            return lista;
        }

        private void recalcularValores()
        {
            cant_instancias = dataGridView1.RowCount;
            cant_columnas = encabezado.Count;
            valores_faltantes = valoresFaltantes();

            int cant_faltantes = valores_faltantes.Values.Sum();

            lblCantInstancias.Text = ("Cantidad de instancias:\n" + cant_instancias);
            lblCantAtributos.Text = ("Cantidad de atributos:\n" + cant_columnas);
            lblValoresFaltantes.Text = ("Cantidad de valores faltanes:\n" + cant_faltantes + "(" + Convert.ToDecimal((cant_faltantes * 100) / (cant_instancias * cant_columnas)).ToString() + "%" + ")");
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog archivo = new OpenFileDialog
            {
                Filter = "Archivo separado por comas (*.csv)|*.csv|Archivo data(*.data)|*.data",
                Title = "Indica el archivo que deseas abrir"
            };
            if (archivo.ShowDialog().Equals(DialogResult.OK))
            {
                string extension = Path.GetExtension(archivo.FileName);
                instancias = new Dictionary<string, List<string>>();
                encabezado = new Dictionary<string, KeyValuePair<string, string>>();
                cant_instancias = 0;
                cmboxDatos.Items.AddRange(tipos_dato);

                if (extension.Equals(".csv"))
                {
                    cargarCsv(archivo.FileName);
                }
                else if(extension.Equals(".data"))
                {
                    cargarData(archivo.FileName);
                }
                cargarGrid();
                this.archivo = archivo;
                this.properties = archivo;

                recalcularValores();
            }
            else {
                MessageBox.Show("El archivo no se abrio");
            }
        }

        private void guardarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string extension = Path.GetExtension(archivo.FileName);

                if (extension.Equals(".csv"))
                {
                    guardarCsv(archivo.FileName);
                }
                else if(extension.Equals(".data"))
                {
                    //pendiente
                }
            }
            catch(System.NullReferenceException)
            {
                MessageBox.Show("Aún no has abierto archivo");
            }
            

        }
        
        //Guardar propiedades pendiente
        private void guardarPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void cargarCsv(string direccionArchivo)
        {
            MessageBox.Show("Se cargara el archivo con la direccion " + direccionArchivo);
            StreamReader leerCsv = new StreamReader(direccionArchivo);
            
            if (!leerCsv.EndOfStream)
            {
                foreach(string columna in leerCsv.ReadLine().Split(','))
                {
                    encabezado.Add(columna, new KeyValuePair<string, string>(tipoTexto.Key, tipoTexto.Value));
                }

                cant_columnas = encabezado.Count();
                
                while (!leerCsv.EndOfStream)
                {
                    string[] instancia = leerCsv.ReadLine().Split(',');
                    int indice = encabezado.Count - 1;

                    List<KeyValuePair<string, KeyValuePair<string, string>>> iterador = encabezado.ToList();
                    List<string> elementos;

                    cant_instancias++;

                    while(indice >= 0)
                    {
                        try
                        { 
                            for (; indice >= 0;  indice--)
                            {
                                if (instancias.ContainsKey(iterador[indice].Key))
                                {
                                    elementos = instancias[iterador[indice].Key];
                                }
                                else
                                {
                                    elementos = new List<string>();
                                    instancias[iterador[indice].Key] = elementos;
                                }
                                elementos.Add(instancia[indice]);
                            }
                        }
                        catch (System.IndexOutOfRangeException) {
                        
                            if (instancias.ContainsKey(iterador[indice].Key))
                            {
                                elementos = instancias[iterador[indice].Key];
                            }
                            else
                            {
                                elementos = new List<string>();
                                instancias[iterador[indice].Key] = elementos;
                            }
                            elementos.Add("?");

                            indice--;
                        }
                    }
                }

                valores_faltantes = valoresFaltantes();
            }
            else
            {
                MessageBox.Show("Es el final del archivo");
            }
            leerCsv.Close();
        }

        private void cargarData(string direccionArchivo)
        {
            MessageBox.Show("Se cargara el archivo con la direccion " + direccionArchivo);
        }

        //Opcion para guardar las instancias como csv
        private void guardarCsv(string direccionArchivo)
        {
            //StreamWriter escribir = new StreamWriter(direccionArchivo);
            StreamWriter escribir = File.AppendText(direccionArchivo);

            int contador = 1;
            foreach(string columna in encabezado.Keys)
            {
                escribir.Write(columna);

                if(encabezado.Keys.Count() > contador)
                {
                    escribir.Write(',');
                }
                else
                {
                    escribir.WriteLine("");
                }
                contador += 1;
            }

            for(int fila = 0; fila < cant_instancias; fila+=1)
            {
                contador = 1;
                foreach (string columna in encabezado.Keys)
                {
                    escribir.Write(instancias[columna].ElementAt(fila));

                    if (encabezado.Keys.Count() > contador)
                    {
                        escribir.Write(',');
                    }
                    else
                    {
                        escribir.WriteLine("");
                    }
                    contador += 1;
                }
            }

            escribir.Close();
        }

        private void actualizarCsv(string accion)
        {
            //Genera la variante del nombre para guardar el archivo
            DateTime time = System.DateTime.Now;
            string direccionArchivo = archivo.FileName;

            string name = Path.GetFileNameWithoutExtension(direccionArchivo);
            string nuevoNombre = time.ToString("yyyyMMdd_HHmmss");

            //Obtiene la direccion de la carpeta para guardar el archivo
            string carpeta = Path.GetDirectoryName(direccionArchivo);

            //Se crea y abre el nuevo archivo
            string direccion = carpeta + "\\" + nuevoNombre;

            //actualiza el log con la accion que se realizo y le pasa el nombre del archivo.
            actualizarLog(time.ToString("yyyyMMdd_HHmmss"), accion);

            guardarCsv(direccion + Path.GetExtension(direccionArchivo));
        }

        private void actualizarLog(string direccion, string accion)
        {
            StreamWriter escribir = File.AppendText(Path.GetDirectoryName(properties.FileName) + "\\" + Path.GetFileNameWithoutExtension(properties.FileName) + ".log");
            escribir.WriteLine(direccion + " => " + accion);
            escribir.Close();
        }

        private void cargarGrid()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            cmBoxColumnas.Items.Clear();
            foreach(string a in encabezado.Keys)
            {
                DataGridViewTextBoxColumn columna = new DataGridViewTextBoxColumn { HeaderText = a, Name = a, SortMode = DataGridViewColumnSortMode.NotSortable};
                dataGridView1.Columns.Add(columna);

                cmBoxColumnas.Items.Add(a);
            }

            int indice = 0;
            int renglon = 0;

            dataGridView1.Rows.Add(cant_instancias);
            
            while (indice != cant_instancias)
            {
                //renglon = dataGridView1.Rows.Add();
                foreach (string columna in encabezado.Keys)
                {
                    dataGridView1.Rows[renglon].Cells[columna].Value = instancias[columna][indice];
                    if (instancias[columna][indice] == "?")
                    {
                        dataGridView1.Rows[renglon].Cells[columna].Style.BackColor = Color.Red;
                        //valores_faltantes++;
                    }
                }
                indice++;
                renglon++;
            }
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string texto = dataGridView1.Columns[e.ColumnIndex].HeaderText;
            txbNombre.Text = texto;
            txbRegex.Text = encabezado[texto].Value;
            cmboxDatos.SelectedItem = encabezado[texto].Key;
            cmBoxColumnas.SelectedItem = texto;
        }
        
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string texto, textoAnterior;
            try
            {
                texto = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            }
            catch(System.NullReferenceException)
            {
                texto = datoTexto;
            }

            int columna = e.ColumnIndex, fila = e.RowIndex;

            textoAnterior = instancias[dataGridView1.Columns[e.ColumnIndex].HeaderText][e.RowIndex];
            instancias[dataGridView1.Columns[e.ColumnIndex].HeaderText][e.RowIndex] = texto;

            if(texto == "?")
            {
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Red;
            }
            else
            {
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.WhiteSmoke;
            }

            recalcularValores();
            actualizarCsv("editar valor - " + textoAnterior + " => " + texto);
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            List<string> valoresColumnaActuales = new List<string>();
            List<string> valoresColumnaAnteriores = new List<string>();

            string nombre = txbNombre.Text;
            string tipoDato = cmboxDatos.Text;
            string columna = cmBoxColumnas.Text;

            //cargando los valores en una lista para mostrarlos en el mensaje del log
            valoresColumnaAnteriores.Add(columna);
            valoresColumnaAnteriores.Add(encabezado[columna].Key);
            valoresColumnaAnteriores.Add(encabezado[columna].Value);

            valoresColumnaActuales.Add(nombre);
            valoresColumnaActuales.Add(tipoDato);
            valoresColumnaActuales.Add(txbRegex.Text);

            dataGridView1.Columns[cmBoxColumnas.SelectedIndex].HeaderText = nombre;
            dataGridView1.Columns[cmBoxColumnas.SelectedIndex].Name = nombre;

            cmBoxColumnas.Items[cmBoxColumnas.SelectedIndex] = nombre;

            encabezado.Remove(columna);
            encabezado[nombre] = new KeyValuePair<string, string>(tipoDato, txbRegex.Text);


            List<string> instanciaAux = instancias[columna];
            instancias.Remove(columna);
            instancias[nombre] = instanciaAux;

            int faltantesAux = valores_faltantes[columna];
            valores_faltantes.Remove(columna);
            valores_faltantes[nombre] = faltantesAux;

            actualizarCsv("editar columna - " + string.Join("|", valoresColumnaAnteriores) + " => " + string.Join("|", valoresColumnaActuales));
        }

        private void btnInfo_Click(object sender, EventArgs e)//Obteniendo informacion de las instancias
        {
            Dictionary<string,int> valores = valoresFaltantes();
            string info = "";
            foreach (string columna in encabezado.Keys)
            {
                info = info + "\nNombre del atributo: " + columna;
                info += "\nTipo de dato: " + encabezado[columna].Key;
                info += "\nValores faltantes: " + valores[columna] + "(" + Convert.ToDecimal(valores[columna] * 100 / cant_instancias).ToString() + "%" + ")";
                info += "\n\n";
            }
            MessageBox.Show(info);
        }

        private void btnEliminarSeleccionado_Click(object sender, EventArgs e)//Eliminando una instancia
        {
            string mensaje = "Se van a eliminar las instancias seleccionadas ";

            foreach (DataGridViewRow fila in dataGridView1.SelectedRows)
            {
                List<string> atributosInstancia = new List<string>();

                mensaje += fila.Index.ToString() + " ";
                foreach(string columna in encabezado.Keys)
                {
                    atributosInstancia.Add(instancias[columna].ElementAt(fila.Index));
                    instancias[columna].RemoveAt(fila.Index);
                }
                dataGridView1.Rows.RemoveAt(fila.Index);

                recalcularValores();
                actualizarCsv("eliminar instancia - " + string.Join(",", atributosInstancia));
            }
        }

        private void btnEliminarAtributo_Click(object sender, EventArgs e)//Eliminando una columna
        {
            if (string.IsNullOrEmpty(cmBoxColumnas.Text))
            {
                MessageBox.Show("Debes seleccionar la columna a eliminar");
            }
            else
            {
                string columna = cmBoxColumnas.Text;

                if (dataGridView1.Columns.Contains(columna))
                {
                    encabezado.Remove(columna);
                    instancias.Remove(columna);
                    valores_faltantes.Remove(columna);
                    dataGridView1.Columns.Remove(columna);
                    cmBoxColumnas.Items.Remove(columna);

                    txbNombre.Text = "";
                    cmboxDatos.SelectedItem = null;
                    txbRegex.Text = "";
                }

                recalcularValores();
                actualizarCsv("eliminar atributo - " + columna);
            }

        }

        private void btnAgregarInstancia_Click(object sender, EventArgs e)//Crear una nueva instancia de datos
        {
            dataGridView1.Rows.Add();
            foreach(string columna in encabezado.Keys)
            {
                instancias[columna].Add("");
            }
            recalcularValores();
        }
        //Puto el Christian
        //pendiente se debe preguntar con que valor rellenar por defecto? si es asi, habilitar en una ventana emergente
        private void btnAgregarColumna_Click(object sender, EventArgs e)//Creando una nueva columna
        {
            //pendiente revalidar el contenido de la nueva columna con la expresion regular
            string columna = "columnaN";
            encabezado[columna] = new KeyValuePair<string, string>(tipoTexto.Key, tipoTexto.Value);
            instancias[columna] = Enumerable.Repeat("?", cant_instancias).Select(x => x.ToString()).ToList();
            valores_faltantes[columna] = valoresFaltantes().Count();

            cargarGrid();
            recalcularValores();
        }

        private void univariableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(this, encabezado, instancias, valores_faltantes, cant_instancias, cant_columnas, tipos_dato);
            form2.Show();
        }

        private void bivariableToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void cmBoxColumnas_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            if (combo.Focused)
            {
                string texto = cmBoxColumnas.Text;
                txbNombre.Text = texto;
                txbRegex.Text = encabezado[texto].Value;
                cmboxDatos.SelectedItem = encabezado[texto].Key;
            }
        }
    }
}
