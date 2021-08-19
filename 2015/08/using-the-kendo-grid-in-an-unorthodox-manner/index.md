## Using the Kendo Grid in an Unorthodox Manner
#
Recently I worked with a business requirement in which I wanted to leverage the Kendo Grid in an unorthodox manner. In particular, I wanted:

- the grid on the form with other values,
- have a checkbox column along with a ‘check all’ header noting a selected row,
- change the grid values based on a form’s dropdown

Each of the above scenarios are not (as of the time of this writing) straightforward to extrapolate from the scenarios and examples provided in the Telerik documentation.

The requirement is to support a user account creation/management screen for pharmacist login accounts as shown in figure 1. Each pharmacist is associated with a particular clinic (e.g. Walgreens, CVS) and specific locations (e.g. CVS store #1, CVS store #2). When setting up a user account, selecting the clinic refreshes the grid of clinic locations.

[![Figure 1: User Management Screen with a Grid](https://intellitect.com/wp-content/uploads/2015/08/Screen-Shot-2015-08-12-at-12.55.41-PM.png)](https://intellitect.com/wp-content/uploads/2015/08/Screen-Shot-2015-08-12-at-12.55.41-PM.png "Using the Kendo Grid in an Unorthodox Manner")

Figure 1: User Management Screen with a Grid

The reason why I chose the grid as the control to display the clinic locations (as opposed to a multi-select checkbox) is that there are thousands of locations associated per clinic and I wanted to leverage the filter and sort capabilities on potentially multiple columns.

Let’s examine how to implement each requirement.

**Requirement 1: Posting the grid on the form with other values while preserving filtering/sorting capabilities**

The best technique I was able to find that accomplished all of these tasks is described at [https://www.telerik.com/support/code-library/submit-form-containing-grid-along-with-other-input-elements](https://www.telerik.com/support/code-library/submit-form-containing-grid-along-with-other-input-elements).  This binds to local data via Ajax.  This allows for the grid to be sortable/filterable on the client side.  Note that with this technique only the items on the page will be posted back. Practically speaking, this is OK in this scenario as a pharmacist will work in only a few stores within a particular city that can be filtered on.

**Requirement 2: Have a checkbox column along with a ‘check all’ header noting a selected row**

The technique used here is to bind to a boolean value along with a custom javascript:

```javascript
columns.Bound(x => x.IsAssociated).ClientTemplate(
   "<input name=ClinicLocations[#=index(data)#].IsAssociated type='checkbox' value='true' #=IsAssociated==true? checked='checked': ''# class='chkbx'/>").
function checkAll(ele) {
   $(".chkbx").prop("checked", $("#masterCheckBox").prop("checked"));
}
```

**Requirement 3: Change the Grid Based on A Form’s Dropdown**

The technique used here is to override the clinic’s dropdown onchange event

```javascript
.Events(x => x.Change("clinicChanged")) //Submits the form to refresh the grid

function clinicChanged(e) {
   //RefreshClinicLocations is a hidden input value that, when set, will force the back end to go
   //through a special workflow to update the model's ClinicLocation list to match that of the
   //selected Clinic

   $("#RefreshClinicLocations").val(true);
   $("#createUserForm").submit();
}
```

**Complete Code Snippets**

The .cshtml snippet for the dropdown and grid is as follows:

```csharp
<div id="clinicSelect">
       <div>
           <div class="editor-label">
               Select the company the clinician works for
           </div>
           <div id="ClinicSelector" class="editor-field">
               @(Html.Kendo().DropDownListFor(model => model.ClinicId)
                     .HtmlAttributes(new
                     {
                         @id = "clinicId",
                         @style = "width:220px"
                     })
                     .SelectedIndex(0)
                     .Events(x => x.Change("clinicChanged")) 
                         //Submits the form to refresh the grid
                     .DataTextField("Name")
                     .DataValueField("Id")
                     .DataSource(source => { source.Read(read => 
                             { read.Action("AllClinics", "Data"); }); })
                     .Template(@dropDownTemplate))
               @Html.ValidationMessageFor(model => model.ClinicId)
           </div>
       </div>
       <div>
           @\*see https://www.telerik.com/support/code-library/submit-form-containing-grid-along-with-other-input-elements\*@
           @(Html.Kendo().Grid(Model.ClinicLocations)
                 .Name("cliniclocations")
                 .Columns(columns =>
                 {
                     columns.Bound(p => p.Id).
                         ClientTemplate("<input type='hidden' 
                            name=ClinicLocations[#=index(data)#]
                            .Id value='#= Id #' />").Hidden();
                     columns.Bound(x => x.IsAssociated).ClientTemplate(
                         "<input name=ClinicLocations[#=index(data)#]." +
                         "IsAssociated type='checkbox' 
                            value='true' #=IsAssociated==true?" +
                         "checked='checked': ''# class='chkbx'/>").
                          HeaderTemplate(@<text><input type='checkbox'
                           id='masterCheckBox' 
                           onclick=' checkAll(this) ' /></text>).Width(50);
                     columns.Bound(x => x.Name).ClientTemplate("#=Name#
                           <input name=ClinicLocations[#=index(data)#].Name " +
                                   "type='hidden' value='#=Name#'/>");
                     columns.Bound(x => x.Address).ClientTemplate(
                        "#=Address#<input name=ClinicLocations[#=index(data)#]
                         .Address " + "type='hidden' value='#=Address#'/>");
                 }).DataSource(dataSource=>dataSource.Ajax()
                 .ServerOperation(false))
                 .Filterable()
                 )
       </div>
   </div>
```

The javascript functions required are:

```javascript
function onSave() {
   $("#RefreshClinicLocations").val(false);
   $("#createUserForm").submit();
}

function clinicChanged(e) {
   //RefreshClinicLocations is a hidden input value that, when set, 
   //will force the back end to go through a special workflow to 
   //update the model's ClinicLocation list to match that of the selected 
   //Clinic
   
   $("#RefreshClinicLocations").val(true);
   $("#createUserForm").submit();
}

function checkAll(ele) {
   $(".chkbx").prop("checked", $("#masterCheckBox").prop("checked"));
}

function index(dataItem) {
   var data = $("#cliniclocations").data("kendoGrid").dataSource.data();
   return data.indexOf(dataItem);
}
```
