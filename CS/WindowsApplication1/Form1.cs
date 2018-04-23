using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler;

namespace WindowsApplication1 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            schedulerControl1.DayView.TopRowTime = new TimeSpan(10, 0, 0);
        }
        private void schedulerControl1_CustomDrawTimeCell(object sender, DevExpress.XtraScheduler.CustomDrawObjectEventArgs e) {
            if(e.ObjectInfo is TimeCell) {
                TimeCell tc = e.ObjectInfo as TimeCell;
                if(tc.Interval.Start.Hour  >= Convert.ToInt32(tc.Resource.CustomFields["LunchStart"]) && tc.Interval.Start.Hour < Convert.ToInt32(tc.Resource.CustomFields["LunchEnd"])) {
                    tc.Appearance.BackColor = Color.LightBlue;
                }
            }
        }

        private void schedulerControl1_AllowAppointmentConflicts(object sender, DevExpress.XtraScheduler.AppointmentConflictEventArgs e) {
            
            Resource r = schedulerStorage1.Resources.Items.GetResourceById(e.AppointmentClone.ResourceId);
            int startLunchHour = Convert.ToInt32(r.CustomFields["LunchStart"]);
            int endLunchHour = Convert.ToInt32(r.CustomFields["LunchEnd"]);
            TimeInterval lunchTimeInterval = new TimeInterval(e.Appointment.Start.Date.AddHours(startLunchHour), TimeSpan.FromHours(endLunchHour - startLunchHour));
            TimeInterval aptTimeInterval = new TimeInterval(e.AppointmentClone.Start, e.AppointmentClone.End);
            if(lunchTimeInterval.IntersectsWith(aptTimeInterval)){
                e.Conflicts.Add(e.AppointmentClone); 
            }
        }


        SolidBrush msb = new SolidBrush(Color.FromArgb(90, 50, 50, 50));
        
        private void schedulerControl1_Paint(object sender, PaintEventArgs e) {
            Rectangle rect = Rectangle.Empty;
            if(schedulerControl1.ActiveView is DayView) {
                foreach(DayViewColumn column in ((DayViewInfo)schedulerControl1.ActiveView.ViewInfo).Columns) {
                    for(int i = 0; i < column.Cells.Count; i++) {
                        TimeCell tc = column.Cells[i] as TimeCell;
                        if (tc.Interval.Start.Hour >= Convert.ToInt32(tc.Resource.CustomFields["LunchStart"]) && tc.Interval.Start.Hour < Convert.ToInt32(tc.Resource.CustomFields["LunchEnd"]))
                        {
                            if(rect == Rectangle.Empty)
                                rect = tc.Bounds;
                            else
                                rect = Rectangle.Union(rect, tc.Bounds);
                        }
                    }
                     
                        if(rect != Rectangle.Empty)
                            using(Font f = new Font("Arial", rect.Height/3, GraphicsUnit.Pixel))
                                e.Graphics.DrawString("Lunch Time", f, msb, new PointF(rect.X + rect.Width / 2 - f.Size*3, rect.Y + rect.Height / 2 - f.Size / 2));
                        rect = Rectangle.Empty;

                }
            }
        }

        private void Form1_Load(object sender, EventArgs e) {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("LunchStart", typeof(int));
            dt.Columns.Add("LunchEnd", typeof(int));
            dt.Rows.Add(new object[] { 1, "John", 10, 12 });
            dt.Rows.Add(new object[] { 2, "Jane", 12, 14 });
            dt.Rows.Add(new object[] { 3, "Bob", 14, 16 });
            schedulerStorage1.Resources.DataSource = dt;
            schedulerStorage1.Resources.Mappings.Id = "ID";
            schedulerStorage1.Resources.Mappings.Caption = "Name";
            schedulerStorage1.Resources.CustomFieldMappings.Add(new ResourceCustomFieldMapping("LunchStart", "LunchStart"));
            schedulerStorage1.Resources.CustomFieldMappings.Add(new ResourceCustomFieldMapping("LunchEnd", "LunchEnd"));

        }
      
    }
}