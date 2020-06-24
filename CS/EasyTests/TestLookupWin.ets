
#Application NonPersistentObjectsDemoWin

*Action Navigation(Default.Project)
*ProcessRecord 
 Code Name = Project X
*ExecuteEditorAction Killer Feature(New)
*FillForm
 Name = F10
 Progress = 10
*Action Save and Close

*CheckFieldValues
 Killer Feature = F10

*Action Save and Close
*Action Navigation(Default.Project)
*ProcessRecord 
 Code Name = Project X

*CheckFieldValues
 Killer Feature = F10

*CheckTable Features
 RowCount = 1
 Columns = Name, Progress
 Row = F10, 10

*ExecuteEditorAction Group(New)
*FillForm
 Name = X9000
*Action Save and Close

*CheckFieldValues
 Group = X9000

*Action Save and Close
*Action Navigation(Default.Project)
*ProcessRecord 
 Code Name = Project X

*CheckFieldValues
 Group = X9000

