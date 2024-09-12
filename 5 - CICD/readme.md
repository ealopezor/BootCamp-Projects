Readme Practica CICD


* la aplicacion a ejecutar es un sencillo programa en java que imprime Hello world, al ser un lenguaje compilado se puede utilizar el Makefile para realizar los test y ejecuciones en local

* Para lanzar los agentes ejecutar el agents.sh dentro de la carpeta agents este configurara 2 agentes con las imagenes de jenkins/ssh-agent:JDK11 en local, instalaran en cada agente los paquetes Maven, Git, Nano y python.

* Una vez lanzados los Agentes se pueden conectar a traves de Jenkins y configurarlos para conectarnos por SSH

* Para ver la configuracion de los agentes, se puede ver en el archivo nodo_jenkins.doc

* Utilizando blue Ocean he creado un pipeline que ejecuta las acciones dentro de los agentes y devuelve los resultados.

* La configuracion para comprobar los cambios en el repositorio esta configurado y el archivo Checking changes.txt muestra la configuracion y el resultado.

* En el Jenkinsfile se pueden observar los diferentes pasos del pipeline hasta la creacion de un artefacto.

* Con el cronjob script se ha realizado una tarea en Jenkins para comprobar el funcionamiento correcto.

Eduardo Hernandez
Devops 7