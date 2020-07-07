#Application NonPersistentObjectsDemoWeb

*Action Navigation(Default.Project)
*ProcessRecord 
 Code Name = Project X

;; fill data
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
*Action Resources.New
*FillForm
 Name = Resource A
 URI = aaa
 Priority = 1
 Embed = True
*Action OK
*Action Resources.New
*FillForm
 Name = Resource B
 URI = bbb
 Priority = 3
*Action OK
*Action Save
*OptionalAction Close

;; check data
*Action Navigation(Default.Project)
*ProcessRecord 
 Code Name = Project X
*CheckTable Features
 RowCount = 2
 Columns = Name, Progress
 Row = Feature1, 2
 Row = Feature2, 1
*CheckTable Resources
 RowCount = 2
 Columns = Name, Priority, Embed
 Row = Resource A, 1, True
 Row = Resource B, 3, False

;; edit and check
*OptionalAction Edit
*ClearSelection Resources
*SelectRecords Resources
 Columns = Name
 Row = Resource A
; Selecting a row doesn't focus it; use the top row.
*FillForm
 Resources.Name = Resource A1
 Resources.URI = aaa1
*Action Save
*OptionalAction Close

*Action Navigation(Default.Project)
*ProcessRecord 
 Code Name = Project X
*CheckTable Resources
 RowCount = 2
 Columns = Name, Priority, Embed
 Row = Resource A1, 1, True
 Row = Resource B, 3, False
*ClearSelection Resources
*SelectRecords Resources
 Columns = Name
 Row = Resource A1
*CheckFieldValues
 Resources.URI = aaa1

*OptionalAction Edit
*ExecuteTableAction Features
 Name = Feature1
 InlineEdit = ''
*FillRecord Features
 IdentityColumns = Name
 IdentityRow = Feature1
 Columns = Name, Progress
 Values = Feature1a, 20
*ExecuteTableAction Features
 InlineUpdate = ''
*Action Save
*OptionalAction Close

*Action Navigation(Default.Project)
*ProcessRecord 
 Code Name = Project X
*CheckTable Features
 RowCount = 2
 Columns = Name, Progress
 Row = Feature1a, 20
 Row = Feature2, 1

;; edit and check
*OptionalAction Edit
*ProcessRecord Features
 Name = Feature2
 Action = Edit
*FillForm
 Progress = 100
*Action OK
*Action Save
*OptionalAction Close

*Action Navigation(Default.Project)
*ProcessRecord 
 Code Name = Project X
*CheckTable Features
 RowCount = 2
 Columns = Name, Progress
 Row = Feature1a, 20
 Row = Feature2, 100

*OptionalAction Close
