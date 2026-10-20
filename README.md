# HeadToHead_Latelier_GF

Pour lancer l’application il suffit de lancer sur VisualStudio ou VisualStudioCode.\
Ouvrir la solution `HeadToHead_Latelier.API.slnx`.\
Définir `HeadToHead_Latelier` comme projet de démarrage.\
Lancer l'application.\
Ouvrez le fichier  `HeadToHead_Latelier/HeadToHead_Latelier.http` et cliquez sur le lien **Send request** au-dessus de chaque requête pour l'exécuter.

Voici la structure du projet : 
- **HeadToHead_Latelier.API** : projet API (contrôleurs, configuration, point d'entrée `Program.cs`)
- **HeadToHead_Latelier.Common** : modèles de données partagés et interfaces 
- **HeadToHead_Latelier.Reader** : implémentation des services de lecture/écriture 
- **HeadToHead_Latelier.Tests** : tests unitaires
