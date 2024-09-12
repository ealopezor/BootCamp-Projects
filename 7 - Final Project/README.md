
## Introducción

Dependency Track es una herramienta que ayuda a gestionar y rastrear las dependencias de software en un proyecto. Permite identificar y resolver problemas de seguridad y licencias en las dependencias utilizadas en tu código.

Para este proyecto desplegaremos la infraestructura en Google Cloud con terraform.

## Despliegue con Terraform del Cluster de Kubernetes


1. Para inicializar el backend utilizaremos el comando:` terraform init`
2. una vez inicializado el backend, comprobamos que podemos ahcer la planificacion y comprobar si nos da algún error con el comando: ` terraform plan`
3. Comprobamos si tenemos algún error, solucionamos en caso de tenerlos y ejecutamos nuevamente ` terraform plan` 
4. Comprobado que no hay errores, aplicamos el plan con el comando ` terraform apply` confirmamos con ` yes ` si esta todo correcto y esperamos a que el despliegue termine y nos confirme que ha terminado con exito.
5. una vez terminado ingresamos en la consola de Google Cloud y verificamos que todos los recursos se han creado satisfactoriamente.


## Configuracion de Dependency-Track utilizando CLI de Google Cloud

1. En esta parte utilizaremos el *Artifact Registry* de GCP, para esto activaremos el las API correspondientes con el siguiente comando: `gcloud services enable artifactregistry.googleapis.com \
containerscanning.googleapis.com`.

2. Configuramos la región exacta de donde se almacenaran las imágenes: `gcloud config set artifacts/location $GCP_REGION` (Donde $GCP_REGION ha sido configurada previamente como una variable de entorno con la zona en la que se  estará  ejecutando nuestro proyecto).
3. Configuramos el repositorio para la imagen del dependency-track con: ` gcloud artifacts repositories create dependency-track \ --repository-format=docker \ --location=$GCP_REGION`
4. Exportamos como variable de entorno el Registro de GCP: ` export GCP_REGISTRY=$GCP_REGION-docker.pkg.dev/$GCP_PROJECT_ID/dependency-track ` 
5. Configuramos Docker con las credenciales para que podamos enviar las imágenes a nuestro repositorio ` gcloud auth configure-docker $GCP_REGION-docker.pkg.dev`.
6. Descargamos las imágenes de dependency-track tanto el front-end como el API-Server de Docker Hub de la siguiente manera: ` docker pull docker.io/dependencytrack/apiserver:4.7.0
docker tag docker.io/dependencytrack/apiserver:4.7.0 $GCP_REGISTRY/apiserver:4.7.0
docker push $GCP_REGISTRY/apiserver:4.7.0`.
` docker pull docker.io/dependencytrack/frontend:4.7.0
docker tag docker.io/dependencytrack/frontend:4.7.0 $GCP_REGISTRY/frontend:4.7.0
docker push $GCP_REGISTRY/frontend:4.7.0`.

### Despliegue de Dependency track en el clúster de Kubernetes

1. Previamente, crearemos un endpoint ` export DT_DOMAIN="dt.endpoints.${GCP_PROJECT_ID}.cloud.goog"`
2. Generamos la configuración del endpoint: ` cat <<EOF | tee endpoint.yaml`
`swagger: "2.0"`
`info:`
  `title: "Dependency Track"`
  `description: "Dependency Track Service"`
  `version: "1.0.0"`
`paths: {}`
`host: "${DT_DOMAIN}"`
`x-google-endpoints:`
`- name: "${DT_DOMAIN}"`
  `target: "${DT_IP}"`
`EOF `
3. desplegamos el endpoint: ` gcloud endpoints services deploy endpoint.yaml`
4. Configuramos `kubectl` con las credenciales de nuestro cluster: `gcloud container clusters get-credentials dependency-track --region $GCP_REGION`
5. Creamos un namespace para el Dependecy-Track: `kubectl create namespace dependency-track` y cambiamos a este `kubectl config set-context --current --namespace=dependency-track`

### Configuración de una base de datos con PostgreSQL

1. Generaremos 2 secretos y los almacenaremos en el Secret Manager de GCP: 
   
  `cat /dev/urandom | tr -dc 'a-zA-Z0-9' | fold -w 30 | head -n 1 | \`
  `gcloud secrets create dependency-track-postgres-admin \`
  `--data-file=-`

`cat /dev/urandom | tr -dc 'a-zA-Z0-9' | fold -w 30 | head -n 1 | \`
  `gcloud secrets create dependency-track-postgres-user \`
  `--data-file=-`

2. Creamos la variable de entorno con el nombre de la base de datos: `export DT_DB_INSTANCE=dependency-track`
   
3. Creamos la instancia de Cloud SQL: `gcloud sql instances create $DT_DB_INSTANCE \`
  `--region=$GCP_REGION \`
  `--no-assign-ip \`
  `--network=projects/$GCP_PROJECT_ID/global/networks/dependency-track \`
  `--database-version=POSTGRES_14 \`
  `--tier=db-g1-small \`
  `--storage-auto-increase \`
  `--root-password=$(gcloud secrets versions access 1 --secret=dependency-track-postgres-admin)`

4. Configuramos el usuario para la base de datos: `gcloud sql users create dependency-track-user \`
  `--instance=$DT_DB_INSTANCE \`
  `--password=$(gcloud secrets versions access 1 --secret=dependency-track-postgres-user)`

5. Creamos la tabla: `gcloud sql databases create dependency-track \
  --instance=$DT_DB_INSTANCE`

6. una vez generada la base de datos en GCP, procedemos a desplegar el dependecy-track.

### Despliegue de Dependency-Track

1. Desplegamos los manifiestos: `kubectl apply -k .`
2. Esperamos que se apliquen tanto el ingress correspondiente como las cargas de trabajo y monitorizamos `kubectl get pods -w -l app=dependency-track-apiserver`
3. una vez tengamos los PODs como RUNNING 2/2 podemos comprobar las cargas del servidor `kubectl logs -f dependency-track-apiserver-0 dependency-track-apiserver`
4. el despliegue puede tardar unos 15 minutos en estar listo, una vez concluido el proceso de despliegue podremos acceder al endpont que anteriormente  habíamos generado y procedemos con la siguiente fase.
5. Para Acceder a la aplicacion utilizaremos el siguiente enlace: https://dt.endpoints.proyecto-final-devops.cloud.goog/



## Configuración inicial

Una vez que hayas iniciado Dependency Track, necesitarás realizar la configuración inicial:

1. Accede a la interfaz de usuario utilizando la URL proporcionada durante la instalación.
2. Crea una cuenta de administrador ingresando tu dirección de correo electrónico y una contraseña segura.
3. Configura la conexión a la base de datos según tus necesidades.
4. Configura el proveedor de autenticación y autorización (opcional).
5. Configura cualquier otra configuración relevante según tus requisitos.

## Importación de proyectos y dependencias

Dependency Track te permite importar tus proyectos y analizar las dependencias utilizadas en ellos. A continuación, se muestra cómo hacerlo:

1. Accede a la interfaz de usuario de Dependency Track.
2. Navega hasta la sección "Proyectos" o "Projects".
3. Haz clic en "Crear nuevo proyecto" o "Create new project".
4. Proporciona la información requerida, como el nombre del proyecto y una descripción opcional.
5. Selecciona el tipo de análisis que deseas realizar: análisis de dependencias o análisis de componentes.
6. Selecciona el método de importación: archivo BOM (Bill of Materials) o archivo SPDX.
7. Sube el archivo correspondiente que describe las dependencias de tu proyecto.
8. Haz clic en "Importar" o "Import" para iniciar el análisis.
9. Dependency Track procesará el archivo y mostrará los resultados en la interfaz de usuario.

## Análisis de resultados

Una vez que Dependency Track haya procesado las dependencias de tu proyecto, podrás ver los resultados y realizar acciones en consecuencia:

1. Accede a la interfaz de usuario de Dependency Track.
2. Navega hasta la sección que contiene los resultados de tu proyecto.
3. Explora las diferentes pestañas y secciones para ver detalles sobre las dependencias analizadas.
4. Identifica las dependencias con problemas de seguridad o licencias.
5. Toma medidas para resolver los problemas, como actualizar las dependencias a versiones más seguras o compatibles con las licencias de tu proyecto.
6. Marca las dependencias como "ignoradas" si no deseas abordar los problemas en este momento.
7. Realiza un seguimiento regular de los resultados y repite el análisis cuando sea necesario.

## Conclusiones

Dependency Track es una herramienta poderosa para gestionar y rastrear las dependencias de software en tus proyectos. Con este manual, deberías tener una base sólida para comenzar

 a utilizar Dependency Track de manera efectiva.

Recuerda visitar la documentación oficial de Dependency Track para obtener más información sobre las características avanzadas y las mejores prácticas de uso. ¡Buena suerte en tu gestión de dependencias!
