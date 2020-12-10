using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TodoApp
{
    public partial class Form1 : Form
    {
        TaskTable model = new TaskTable();
        public Form1()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        void Clear()
        {
            textTodo.Text = "";
            btnSave.Text = "Save";
            btnDelete.Enabled = false;
            if(checkBox1 != null)
            {
                checkBox1.Checked = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Clear();
            FillTheDataGridView();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            model.Deadline = dateTimePicker1.Value;
            model.Task = textTodo.Text.Trim();
            model.Done = checkBox1.Checked;

            using (TodoEntities db = new TodoEntities())
            {
                if (model.id == 0)
                    db.TaskTable.Add(model);
                else
                    db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }

            Clear();
            FillTheDataGridView();
            MessageBox.Show("Hinzugefügt");
            
        }

        void FillTheDataGridView()
        {
            using(TodoEntities db = new TodoEntities())
            {
                dgvTodo.DataSource = db.TaskTable.ToList<TaskTable>();
            }
        }

        private void dgvTodo_DoubleClick(object sender, EventArgs e)
        {
            if(dgvTodo.CurrentRow.Index != -1)
            {
                model.id = Convert.ToInt32(dgvTodo.CurrentRow.Cells["id"].Value);
                using (TodoEntities db = new TodoEntities())
                {
                    model = db.TaskTable.Where(x => x.id == model.id).FirstOrDefault();
                    textTodo.Text = model.Task;
                    dateTimePicker1.Value = model.Deadline.Date;
                    checkBox1.Checked = model.Done;
                        
                }
                btnSave.Text = "Update";
                btnDelete.Enabled = true;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Weiter mit löschen", "EF CRUD Operation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            using(TodoEntities db = new TodoEntities ())
            {
                    var entry = db.Entry(model);
                    if (entry.State == EntityState.Detached)
                        db.TaskTable.Attach(model);
                    db.TaskTable.Remove(model);
                    db.SaveChanges();
                    FillTheDataGridView();
                    Clear();
            }
        }
    }
}
