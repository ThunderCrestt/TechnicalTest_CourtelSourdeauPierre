# TechnicalTest_CourtelSourdeauPierre
Test technique de Courtel--Sourdeau Pierre

Pour ce test j'ai supposé que chaque rotator sélectionné par l'utilisateur allait être modifié, j'ai donc choisi de ne pas mettre de checkbox dans la liste des rotators à modifier.
De plus j'ai choisi de mettre à jour les valeurs de la fenêtre ( editorWindow ) en fonction du dernier rotator de la liste.
J'ai aussi supposé que si l'utilisateur ouvre la fenêtre d'édition depuis l'inspecteur, cela rajoute le rotator sélectionné dans la liste d'édition et donc que la fenêtre affiche ses valeurs.
J'ai aussi décidé de bloquer la validation des changements dans certains cas, par exemple quand aucune checkboxe n'est cochées, quand aucun rotator n'est dans la liste ou encore quand aucun object to rotate n'est sélectionné dans les rotations settings.
J'ai donc aussi ajouté des warnings lorsque ces cas surviennent.
