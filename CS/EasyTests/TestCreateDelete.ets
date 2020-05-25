#Application NonPersistentObjectsDemoWin
#Application NonPersistentObjectsDemoWeb

*Action Navigation(Default.Project)
*ProcessRecord 
 Code Name = Project X
*OptionalAction Edit
*FillForm
 Group = XHD
*Action Save
*Action Features.New
*FillForm
 Name = Feature1
 Progress = 2
#IfDef NonPersistentObjectsDemoWin
*Action Save and Close
#EndIf
#IfDef NonPersistentObjectsDemoWeb
*Action OK
#EndIf
*Action Features.New
*FillForm
 Name = Feature2
 Progress = 1
#IfDef NonPersistentObjectsDemoWin
*Action Save and Close
#EndIf
#IfDef NonPersistentObjectsDemoWeb
*Action OK
#EndIf
*Action Save
*OptionalAction Close
*Action Navigation(Default.Project)

*ProcessRecord 
 Code Name = Project X

*CheckFieldValues
 Group = XHD
*CheckTable Features
 RowCount = 2
 Columns = Name, Progress
 Row = Feature1, 2
 Row = Feature2, 1

*OptionalAction Edit
*Action Resources.New
;*ClearSelection Resources
;*SelectRecords Resources
; Columns = Name
; Row = ''
#IfDef NonPersistentObjectsDemoWin
*FillForm
 Resources.Name = Resource A
 Resources.URI = aaa
 Resources.Priority = 1
 Resources.Embed = True
#EndIf
#IfDef NonPersistentObjectsDemoWeb
*FillForm
 Name = Resource A
 URI = aaa
 Priority = 1
 Embed = True
*Action OK
#EndIf
*Action Resources.New
;*ClearSelection Resources
;*SelectRecords Resources
; Columns = Name
; Row = ''
#IfDef NonPersistentObjectsDemoWin
*FillForm
 Resources.Name = Resource B
 Resources.URI = bbb
#EndIf
#IfDef NonPersistentObjectsDemoWeb
*FillForm
 Name = Resource B
 URI = bbb
*Action OK
#EndIf
*Action Save
*OptionalAction Close

*Action Navigation(Default.Project)
*ProcessRecord 
 Code Name = Project X

#IfDef NonPersistentObjectsDemoWin
*CheckTable Resources
 RowCount = 2
 Columns = Name, URI, Priority, Embed
 Row = Resource A, aaa, 1, True
 Row = Resource B, bbb, 0, False
 CheckInvisibleColumns = True
#EndIf
#IfDef NonPersistentObjectsDemoWeb
*CheckTable Resources
 RowCount = 2
 Columns = Name, Priority, Embed
 Row = Resource A, 1, True
 Row = Resource B, 0, False
#EndIf

*OptionalAction Edit
*ClearSelection Features
*SelectRecords Features
 Columns = Name
 Row = Feature2
*Action Features.Delete
*HandleDialog
 Respond = Yes

*CheckTable Features
 RowCount = 1
 Columns = Name
 Row = Feature1

*ClearSelection Resources
*SelectRecords Resources
 Columns = Name
 Row = Resource A
*Action Resources.Delete
*HandleDialog
 Respond = Yes

*CheckTable Resources
 RowCount = 1
 Columns = Name
 Row = Resource B

*Action Save
*OptionalAction Close
*Action Navigation(Default.Project)
*ProcessRecord 
 Code Name = Project X

*CheckTable Features
 RowCount = 1
 Columns = Name
 Row = Feature1
*CheckTable Resources
 RowCount = 1
 Columns = Name
 Row = Resource B

