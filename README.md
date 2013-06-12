SurroundWithTransactionAddin
============

# Disclaimer
The following document and all the code in this repository are not part of the LoadRunner product!
You are using the code and data and the products generated by compiling this code at your own risk.
***
The SurroundWithTransactions ```addin``` is based on the great examples provided by [BorisKozo](https://github.com/BorisKozo/XmlViewAddin) 

# General
The SurroundWithTransaction ```addin``` will allow you to select steps in a script and quickly wrap them in a transaction. Right click
on a single step or select a few steps and right click on the selection
![The selected steps in VuGen and the new menu item 'Surround with Transaction'](/img/surroundwith1.png "The selected steps in VuGen and the new menu item 'Surround with Transactions'")

Selecting the 'Surround with Transactions' menu will bring up the 'End transaction' dialog. 
Enter the transaction name and status.
![Enter the name of the transaction](/img/surroundwith2.png "Enter the name of the transaction")

The result is that the selected steps are wrapped in a new transaction.
![Enter the name of the transaction](/img/surroundwith3.png "The selected steps are wrapped in a new transaction")

#Compilation Note
To compile the project you must add *%VUGEN_PATH%\bin* to the reference paths of the project.
There is a compiled version in the *bin* directory in the root of the repository. If you 
don't want to compile yourself you can copy the content of this directory to *%VUGEN_PATH%\addins\extra\SurroundWithTransaction*.

# Conclusions

