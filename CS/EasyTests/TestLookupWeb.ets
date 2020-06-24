
#Application NonPersistentObjectsDemoWeb

*Action Navigation(Default.Project)
*ClearSelection Project
*SelectRecords Project
 Columns = Code Name
 Row = Project X
*Action Edit
*ExecuteEditorAction Killer Feature(New)
*FillForm
 Name = F10
 Progress = 10
*Action OK

*CheckFieldValues
 Killer Feature = F10

*Action Save
*Action Navigation(Default.Project)
*ProcessRecord 
 Code Name = Project X

*CheckFieldValues
 Killer Feature = F10

*CheckTable Features
 RowCount = 1
 Columns = Name, Progress
 Row = F10, 10

*Action Edit
*ExecuteEditorAction Group(New)
*FillForm
 Name = X9000
*Action OK

*CheckFieldValues
 Group = X9000

*Action Save
*Action Navigation(Default.Project)
*ProcessRecord 
 Code Name = Project X

*CheckFieldValues
 Group = X9000

