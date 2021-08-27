

## Integrations and the Canoncial Model
#
**What is a canonical model?**

A canonical model is a way to define a logical data model for a particular business object. Rather, a standard which defines how the data will be constructed. It should be as detailed as possible, and represent all of the entities and relationships which are used to construct the data. The model  will become the template for how data is exchanged between systems and services. An organization will eventually identify a number of canonical models, ideally with minimal overlap. There is some debate as to which form the canonical model should live, but for now, I will assume the canonical model will be maintained in XML and bound by an XML Schema Definition (XSD).

**But what if I don't have a canonical model?**

Don't panic! You are not alone. In this case, my recommendation is to choose the schema from the endpoint system identified as the system of record (SoR). Choosing the SoR's schema will ensure, at a minimum, all of the possible data available for the given entity. In theory, the SoR should be providing all possible attributes of the entity. All subscribers to this data must use the information provided by the SoR. There are cases where you may need to perform data enrichment, by collecting additional information along the way, but let's not complicate things quite yet.

**Real world example…**

Consider you have the following applications in your enterprise: Enterprise Resource Planning (ERP) and Customer Information System (CIS). In this example, you need to send Accounts Payable (AP) information from your CIS system to your ERP system. If both applications were built by the same vendor, it is possible a productized integration may exist. This means the source system will natively produce the data in a format the target system will also natively consume. Therefore, you don't have to customize the integration, or rather, the data being integrated. If that is not the case, you are faced with defining an integration that will support at least two interfaces: output from the source system and input to the target system.

**I've got an apple, and you want oranges…**

In the above example, let's pretend the CIS system exchanges its AP data in the form of an apple, and the ERP system in the form of an orange. In order to send data from one system to the other, we will need to convert apples to oranges, or the reverse. In order to accomplish this magical feat, we will need to implement some sort of transformation. The initial desire may be to directly transform the SoR's schema to each of the subscribers, but I strongly caution that approach. In development terms, this sounds easy enough, but don't get caught creating a point-to-point integration. Point-to-point integrations impose a tight coupling between systems.  When one system changes, a change will be required with the “other” system, even if originating change has no impact on the “other” system.

[![Screen Shot 2015-09-23 at 4.51.09 PM](https://intellitect.com/wp-content/uploads/2015/09/Screen-Shot-2015-09-23-at-4.51.09-PM.png)](https://intellitect.com/wp-content/uploads/2015/09/Screen-Shot-2015-09-23-at-4.51.09-PM.png "Integrations and the Canonical Model")

 

 

 

 

 

Instead, it is better to create your own canonical schema based on the SoR, but which lives in a different namespace. This will help manage changes to the SoR's schema, which may have no impact on any downstream mapping processes.

[![Screen Shot 2015-09-23 at 4.51.25 PM](https://intellitect.com/wp-content/uploads/2015/09/Screen-Shot-2015-09-23-at-4.51.25-PM.png)](https://intellitect.com/wp-content/uploads/2015/09/Screen-Shot-2015-09-23-at-4.51.25-PM.png "Integrations and the Canonical Model")

Assumptions:

- System A is the SoR
- System B is a subscriber of the data provided by System A
- System C is a subscriber of the data provided by System A

Example - When the SoR's schema is the canonical model

- Mapping between System A and System B
- Mapping between System A and System C
- If System A's schema changes, you must modify both maps to Systems B and C regardless if new data or mapping rules are presented

Example (**_Preferred_**) - When a discrete canonical model is used

- Mapping between System A and the Canonical Model
- Mapping between the Canonical Model and System B
- Mapping between the Canonical Model and System C
- If System A's schema changes, you may only need to modify the map to the Canonical Model, mappings from the Canonical Model to Systems B and C will only occur if new data or mapping rules are presented

**In summary…**

The use of canonical models will help you define the data being transported between systems. It will contain all possible attributes and relationships important to your business. They will also allow you to more loosely couple your integrations, and provide minimal impact when endpoint systems change the format of the data they either publish or consume.
