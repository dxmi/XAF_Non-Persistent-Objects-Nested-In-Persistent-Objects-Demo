#Application NonPersistentObjectsDemoWeb

*Action Navigation(Default.Project)
*ProcessRecord 
 Code Name = Project X
*OptionalAction Edit
*Action Features.New
*FillForm
 Name = Feature1
 Progress = 2
*Action OK
*Action Features.New
*FillForm
 Name = Feature2
 Progress = 1
*Action OK
*Action Save
*OptionalAction Close
*Action Navigation(Default.Project)

*ProcessRecord 
 Code Name = Project X

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
*FillForm
 Name = Resource A
 URI = aaa
 Priority = 1
 Embed = True
*Action OK
*Action Resources.New
;*ClearSelection Resources
;*SelectRecords Resources
; Columns = Name
; Row = ''
*FillForm
 Name = Resource B
 URI = bbb
*Action OK
*Action Save
*OptionalAction Close

*Action Navigation(Default.Project)
*ProcessRecord 
 Code Name = Project X

*CheckTable Resources
 RowCount = 2
 Columns = Name, Priority, Embed
 Row = Resource A, 1, True
 Row = Resource B, 0, False

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

