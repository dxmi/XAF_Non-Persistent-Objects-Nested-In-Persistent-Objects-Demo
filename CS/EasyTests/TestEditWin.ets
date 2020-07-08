#Application NonPersistentObjectsDemoWin

*Action Navigation(Default.Project)
*ProcessRecord 
 Code Name = Project X
*OptionalAction Edit

;; fill data
*OptionalAction Edit
*Action Features.New
*FillForm
 Name = Feature1
 Progress = 2
*Action Save and Close
*Action Features.New
*FillForm
 Name = Feature2
 Progress = 1
*Action Save and Close
*Action Resources.New
*ClearSelection Resources
*SelectRecords Resources
 Columns = Name
 Row = ''
*FillForm
 Resources.Name = Resource A
 Resources.URI = aaa
 Resources.Priority = 1
 Resources.Embed = True
*Action Resources.New
*ClearSelection Resources
*SelectRecords Resources
 Columns = Name
 Row = ''
*FillForm
 Resources.Name = Resource B
 Resources.URI = bbb
*Action Save
*Action Close

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
 Columns = Name, URI, Priority, Embed
 Row = Resource A, aaa, 1, True
 Row = Resource B, bbb, 0, False
 CheckInvisibleColumns = True

;; edit and check
*ClearSelection Resources
*SelectRecords Resources
 Columns = Name
 Row = Resource A
*FillForm
 Resources.Name = Resource A1
 Resources.URI = aaa1
*Action Save
*Action Close

*ProcessRecord 
 Code Name = Project X
*ClearSelection Resources
*SelectRecords Resources
 Columns = Name
 Row = Resource A1
*CheckTable Resources
 RowCount = 2
 Columns = Name, URI, Priority, Embed
 Row = Resource A1, aaa1, 1, True
 Row = Resource B, bbb, 0, False
 CheckInvisibleColumns = True

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
*Action Close

*ProcessRecord 
 Code Name = Project X
*CheckTable Features
 RowCount = 2
 Columns = Name, Progress
 Row = Feature1a, 20
 Row = Feature2, 1

;; edit and check
*ProcessRecord Features
 Name = Feature2
*FillForm
 Progress = 100
*Action Save
*Action Close
*Action Save
*Action Close

*ProcessRecord 
 Code Name = Project X
*CheckTable Features
 RowCount = 2
 Columns = Name, Progress
 Row = Feature1a, 20
 Row = Feature2, 100

*Action Close
