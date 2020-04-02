Below is our guidance for how to report issues, propose new features, and submit contributions via Pull Requests (PRs).

# Testing

To validate your pull request, your changes have to pass the complete generation and JSON files must be readable by any serializable:

    program ".\SC_3.8.0\Data" ".\SC_3.8.0\Data.json" --all

# Submitting changes

Please follow our coding conventions (below) and prefer atomic commits (one feature per commit).

Always write a clear log message for your commits. One-line messages are fine for small changes, but bigger changes should look like this:

    $ git commit -m "A brief summary of the commit
    > 
    > A paragraph describing what changed and its impact."

# Coding conventions

- We indent using four spaces (hard tabs).
- We ALWAYS put spaces after list items and method parameters ([1, 2, 3], not [1,2,3]), around operators (x += 1, not x+=1).
- Use the `**Logger**` class to print messages.
- This is open source software, consider the people who will read your code.

# How to add a new type

In the project: **StarCitizen XML To JSON.**

First, add your new type to the enum `SharedProject.SCType` and increment its value like so:

    public enum SCType
    {
    	None = 0,
    	Ship = 1 << 1,
    	New_Type = 1 << 2, // ADDED
    	
    	Every = (New_Type * 2) - 1, // EDITED
    }

Never edit other values

Next, you have to tell how to detect your new type, go in the `CryXML` class and in the static function `DetectType`. At the end of the function add a simple if condition which return your new type:

    private static SCType DetectType(XmlDocument xfile)
    {
    	if(xfile.SelectSingleNode("/*").Name.Equals("Vehicle"))
    		return SCType.Ship;
    
    	// Detect using the path
    	if (new FileInfo(xfile.BaseURI).Directory.FullName.ToLower().Contains("amazing\\folders"))
    		return SCType.New_Type;
    
    	return SCType.None;
    }

Perfect, your type can be detected by the program, now you have to create your class which will process our data, you can simply copy/paste the existing template called `JEntitySample.cs` and place it in a subfolder with the name of your type (don't forget to rename the class like "`JNewType`" )

You can now create your logic in the `Process()`, the Process function is called for every files found corresponding to your rule (in `DetectType`). You have to call `base.WriteFile()` to register and write a file to the disk.

(See other files to understand the converting process)

Use your new type in the program, you have some modifications to do:

- **CryXML.ConvertJSON():** Add a switch case, casting to your type:

        case SCType.New_Type:
        	jObject = new JNewType(file, destination, source);
        	break;

- **Program.FindParameters:** Add a switch case to handle your type:

        case "--new-type":
        case "-nt":
        	parameters |= SCType.New_Type
        	break;

- **Program.PrintHelp**: Add to the help message.

## (Optional) Add support in the **JSON To SQL**

In the project: **StarCitizen JSON To SQL.**

Some small changes:

- **CryJSON.DetectType**: Add your type detection.

        case "NewType":
        	return SCType.New_Type;

- **Program.FindParameters:** Add a switch case to handle your type:

        case "--new-type":
        case "-nt":
        	parameters |= SCType.New_Type
        	break;

- **Program.PrintHelp**: Add to the help message.
