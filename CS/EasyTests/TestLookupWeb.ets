
#Application NonPersistentObjectsDemoWeb

*Action Navigation(Default.Product)
*ProcessRecord 
 Name = Product 2
*OptionalAction Edit
*FillForm
 Group = XHD
*Action Save
*OptionalAction Close

*Action Navigation(Default.Product)
*ProcessRecord 
 Name = Product 2
*CheckFieldValues
 Group = XHD
*OptionalAction Close

;New

*Action Navigation(Default.Product)
*ClearSelection Product
*SelectRecords Product
 Columns = Name
 Row = Product 2
*Action Edit
*ExecuteEditorAction Group(New)
*FillForm
 Name = X9000
*Action OK

*CheckFieldValues
 Group = X9000

*Action Save
*Action Navigation(Default.Product)
*ProcessRecord 
 Name = Product 2

*CheckFieldValues
 Group = X9000

