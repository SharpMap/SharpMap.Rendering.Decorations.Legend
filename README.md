SharpMap.Rendering.Decorations.Legend
=====================================

A legend solution for SharpMap

Step 1: Create your map

Step 2: Create your legend for the map
```C#
var factory = new SharpMap.Rendering.Decoration.Legend.Factories.LegendFactory
    {
        HeaderFont = new System.Drawing.Font("Times New Roman", 24),
        SymbolSize = new System.Drawing.Size(14, 14);
        Indentation = 7;
        //more properties are ItemSize, ForeColor; Padding
    };
    
var legend = factory.Create(map);
// Get a legend image
var legendImage = map.GetLegendImage();

// Display the legend image on the map
map.Decorations.Add(legend);
    
```
