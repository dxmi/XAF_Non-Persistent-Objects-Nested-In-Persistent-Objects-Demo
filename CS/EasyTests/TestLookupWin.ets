
#Application NonPersistentObjectsDemoWin

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
*ProcessRecord 
 Name = Product 2
*ExecuteEditorAction Group(New)
*FillForm
 Name = X9000
*Action Save and Close

*CheckFieldValues
 Group = X9000

*Action Save and Close
*Action Navigation(Default.Product)
*ProcessRecord 
 Name = Product 2

*CheckFieldValues
 Group = X9000

