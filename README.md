# Sap2000-API
Uses the Sap2000 Open Api to get the models information into an object oriented format using VB.Net



#Requirements
In order for this program to run CSI SAP 2000 V16 must be installed on the system. The program should work for other SAP versions, by either:
<br>1-	Replacing all instances of <code>SAP2000v16 </code> with the appropriate namespace.
<br>or
<br>2-	Changing the code from early binding to late binding. 

#SapStructure#
#### Constructor ####
The full path to one of the sap models is passed to the constructor and information regarding the model is retrieved. Note that force values will not be retrieved

#### GetResults ####
Retrieves analysis results from the input sap model. Note you can call this function multiple times with different models and all the forces will be added to the object under different model names.

#### SaveObject ####
The program may take along time opening and closing multiple sap files. Using this function you can save the object before exiting. The next time you run the program you can call Open object to load the SapStructure object.

#### OpenObject ####
See SaveObject

#### CloseApplication ####
Will close any open sap models. 

#### GetElement ####
Returns reference to the an element object based on the input ID

#Element#
This object contains information regarding a single element in sap

#### Dir ####
The direction of the element, X, Y, Z or Diagonal

#### Forces ####
A list of force objects. 

#### Joint1 ####
The first joint of the element. Usually the node with the smaller coordinate values. 

#### Joint2 ####
The second joint of the element. Usually the node with larger coordinate values. 

#### Section ####
The name of the secon assigned to the element

#### ID ####
The ID of the element as in SAp

#### t2 ####
The t2 dimension of the section assigned to the element

#### t3 ####
The t3 dimension of the section assigned to the element

#### Type ####
The type of element. Beam or Column. This depends on the design method selected for the section. If column is selected this will be column. If beam is selected then this will be beam. 


#Joint#
Contains information about a single joint in SAP.

#### X ####
The X coorindate of the joint

#### Y ####
The Y coorindate of the joint

#### Z ####
The Z coorindate of the joint

#### ID ####
The ID of the joint as in the sap model

#### Connectivity ####
A list of element objects which are connected to the joint. 

# Force #
Contains information about the force at a single station of the element

#### ElementID ####
The Id of the element the force object is assigned to

#### M2, M3, P, T, V2, V3 ####
List of ForceType objects associated with the different force at the section

#### Station ####
The distance of the station from the start of the element.

# ForceType #
Contains information about the force at a single station of the element for a specific type (for example V2 or V3 or M2, ...)

#### AddForce ####
This function is used to add a new force value to the list of forces. 

#### Cases ####
List of Force_LoadCase objects for the different load case forces. 

#### Combos ####
List of Force_LoadCase objects for the different combo forces. 

#### MaxAbs ####
Returns the maximum value of the force regardless of the sign

# Force_LoadCase #
Contains information about the force at a single station of the element for a specific type for a specific Load case or combination. 

#### LoadCase ####
The name of the loadcase or combination the force is associated with

#### ForceInfos ####
List of Force_Load_Info objects. 

# Force_Load_Info #
Contains information about the force at a single station of the element for a specific type for a specific Load case or combination for a specific model. Note that analysis results can be retrieved from multiple models.  


# License
[The MIT License (MIT)](http://opensource.org/licenses/MIT)
