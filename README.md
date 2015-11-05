# Sap2000-API
Uses the Sap2000 Open Api to get the models information into an object oriented format using VB.Net



#Requirements
In order for this program to run CSI SAP 2000 V16 must be installed on the system. The program should work for other SAP versions, by either:
<br>1-	Replacing all instances of <code>SAP2000v16 </code> with the appropriate namespace.
<br>or
<br>2-	Changing the code from early binding to late binding. 

#SapStructure#
##Constructor##
The full path to one of the sap models is passed to the constructor and information regarding the model is retrieved. Note that force values will not be retrieved

##GetResults##
Retrieves analysis results from the input sap model. Note you can call this function multiple times with different models and all the forces will be added to the object under different model names.

##SaveObject##
The program may take along time opening and closing multiple sap files. Using this function you can save the object before exiting. The next time you run the program you can call Open object to load the SapStructure object.

##OpenObject##
See SaveObject

##CloseApplication##
Will close any open sap models. 

##GetElement##
Returns reference to the an element object based on the input ID

#Element#
This object contains information regarding a single element in sap

##Dir##
The direction of the element, X, Y, Z or Diagonal

##Forces##
A list of force objects. 






#Output
The SapStructure object provides 3 output:
<br><b>Forces:</b> This will be a list of force objects. Each object will be at one station of the cast unit. Note that there are multiple models and multiple load cases in each station. 
<br>The following code will print out the shear in the V2 direction at the first station of the cast unit for all models and load cases:
 <pre>
  'loop through load cases
  For i As Integer = 0 To Me.Forces.Item(0).V2.Cases.Count - 1
      'loop through models
      For j As Integer = 0 To Me.Forces.Item(0).V2.Cases.Item(i).Force_Infos.Count - 1
          Console.WriteLine(Me.Forces.Item(0).V2.Cases.Item(i).Force_Infos.Item(j).Value)
      Next
  Next
</pre>

<br><b>T3:</b> The T3 dimension of the cast unit at each station.
<br><b>T2:</b> The T2 dimension of the cast unit at each station.

#Output
This output below was generated by the program above. 
- **Type:** The type was specified to be a beam
- **Direction:** The beam direction was in the X direction
- **Elevation:** This is the Z parameter.
- **Y:** The beam was in the X direction
- **Element:** The values in this row specify the element being printed at each station. Note that each column is the value at a station
- **Max(row 9):** The maximum value of the V2 shear among all models for the DEAD load case
 

![Output](http://jtech-online.com/wp-content/uploads/2015/10/Output.png)


# License
[The MIT License (MIT)](http://opensource.org/licenses/MIT)
