Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports DevExpress.XtraScheduler.Drawing
Imports DevExpress.XtraScheduler

Namespace WindowsApplication1
    Partial Public Class Form1
        Inherits Form

        Public Sub New()
            InitializeComponent()
            schedulerControl1.DayView.TopRowTime = New TimeSpan(10, 0, 0)
        End Sub
        Private Sub schedulerControl1_CustomDrawTimeCell(ByVal sender As Object, ByVal e As DevExpress.XtraScheduler.CustomDrawObjectEventArgs) Handles schedulerControl1.CustomDrawTimeCell
            If TypeOf e.ObjectInfo Is TimeCell Then
                Dim tc As TimeCell = TryCast(e.ObjectInfo, TimeCell)
                If tc.Interval.Start.Hour >= Convert.ToInt32(tc.Resource.CustomFields("LunchStart")) AndAlso tc.Interval.Start.Hour < Convert.ToInt32(tc.Resource.CustomFields("LunchEnd")) Then
                    tc.Appearance.BackColor = Color.LightBlue
                End If
            End If
        End Sub

        Private Sub schedulerControl1_AllowAppointmentConflicts(ByVal sender As Object, ByVal e As DevExpress.XtraScheduler.AppointmentConflictEventArgs) Handles schedulerControl1.AllowAppointmentConflicts

            Dim r As Resource = schedulerStorage1.Resources.Items.GetResourceById(e.AppointmentClone.ResourceId)
            Dim startLunchHour As Integer = Convert.ToInt32(r.CustomFields("LunchStart"))
            Dim endLunchHour As Integer = Convert.ToInt32(r.CustomFields("LunchEnd"))
            Dim lunchTimeInterval As New TimeInterval(e.Appointment.Start.Date.AddHours(startLunchHour), TimeSpan.FromHours(endLunchHour - startLunchHour))
            Dim aptTimeInterval As New TimeInterval(e.AppointmentClone.Start, e.AppointmentClone.End)
            If lunchTimeInterval.IntersectsWith(aptTimeInterval) Then
                e.Conflicts.Add(e.AppointmentClone)
            End If
        End Sub


        Private msb As New SolidBrush(Color.FromArgb(90, 50, 50, 50))

        Private Sub schedulerControl1_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles schedulerControl1.Paint
            Dim rect As Rectangle = Rectangle.Empty
            If TypeOf schedulerControl1.ActiveView Is DayView Then
                For Each column As DayViewColumn In CType(schedulerControl1.ActiveView.ViewInfo, DayViewInfo).Columns
                    For i As Integer = 0 To column.Cells.Count - 1
                        Dim tc As TimeCell = TryCast(column.Cells(i), TimeCell)
                        If tc.Interval.Start.Hour >= Convert.ToInt32(tc.Resource.CustomFields("LunchStart")) AndAlso tc.Interval.Start.Hour < Convert.ToInt32(tc.Resource.CustomFields("LunchEnd")) Then
                            If rect = Rectangle.Empty Then
                                rect = tc.Bounds
                            Else
                                rect = Rectangle.Union(rect, tc.Bounds)
                            End If
                        End If
                    Next i

                        If rect <> Rectangle.Empty Then
                            Using f As New Font("Arial", rect.Height\3, GraphicsUnit.Pixel)
                                e.Graphics.DrawString("Lunch Time", f, msb, New PointF(rect.X + rect.Width \ 2 - f.Size*3, rect.Y + rect.Height \ 2 - f.Size / 2))
                            End Using
                        End If
                        rect = Rectangle.Empty

                Next column
            End If
        End Sub

        Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim dt As New DataTable()
            dt.Columns.Add("ID", GetType(Integer))
            dt.Columns.Add("Name", GetType(String))
            dt.Columns.Add("LunchStart", GetType(Integer))
            dt.Columns.Add("LunchEnd", GetType(Integer))
            dt.Rows.Add(New Object() { 1, "John", 10, 12 })
            dt.Rows.Add(New Object() { 2, "Jane", 12, 14 })
            dt.Rows.Add(New Object() { 3, "Bob", 14, 16 })
            schedulerStorage1.Resources.DataSource = dt
            schedulerStorage1.Resources.Mappings.Id = "ID"
            schedulerStorage1.Resources.Mappings.Caption = "Name"
            schedulerStorage1.Resources.CustomFieldMappings.Add(New ResourceCustomFieldMapping("LunchStart", "LunchStart"))
            schedulerStorage1.Resources.CustomFieldMappings.Add(New ResourceCustomFieldMapping("LunchEnd", "LunchEnd"))

        End Sub

    End Class
End Namespace