# **Practica final modulo contenedores mas que VMs**

Se ha creado un repositorio con diferentes carpetas donde se incluyen los archivos para desplegar una aplicacion sencilla en docker, Kubernetes y utilizar charts de Helm.

$$
1.
$$

En la carpeta **fuente** se encuentran los ficheros para realizar la imagen de la aplicacion con docker, se ha utilizado una pequeña aplicacion en python la cual tambien se encuentra en esta carpeta.

Utilizando el Dockerfile obtenemos una imagen bastante pequeña, esta imagen se ha alojado en Docker hub para su posterior utilizacion y puede ser descargada desde el repositorio utilizando el comando en la terminal `docker pull ealopezor/app:v1.0.0`.

En esta carpeta tambien se encuentra un fichero Docker-compose para levantar el microservicio en local utilizando un contendor con redis




$$
2.
$$

En la siguiente carpeta **k8s** se encuentran los archivos para desplegar la aplicacion en kubernetes.

hemos utilizado un volumen persistente para mantener los datos correspondientes a la base de datos para ellos levantamos el archivo redis-pvc.yaml

se ha generado un secreto para poder acceder a nuestra aplicacion utilizando el siguiente comando

```console
`kubectl create secret generic redis --from-literal=password=secret`
```

El servicio sera de tipo ClusteIP, desplegandolo con 

```console
`kubectl apply -f app-svc.yaml`
```

Levantaremos el Ingress controller para exponer nuestra aplicacion a internet, para ello utilizaremos el 

```console
kubectl apply -f app-ingress.yaml
```

Aplicaremos los mismos comandos para levantar los manifiestos de nuestro redis.


En la carpeta **app** se encuentran los charts de helm que utilizaremos para desplegar utilizando el gestor de paquetes de kubernetes Helm.

para desplegar entramos en la carpeta app

**el chart principal contiene los siguentes detalles:**

```json
apiVersion: v2
name: app
description: Practica Contenedores mas que VMs Devops 7

# A chart can be either an 'application' or a 'library' chart.
#
# Application charts are a collection of templates that can be packaged into versioned archives
# to be deployed.
#
# Library charts provide useful utilities or functions for the chart developer. They're included as
# a dependency of application charts to inject those utilities and functions into the rendering
# pipeline. Library charts do not define any templates and therefore cannot be deployed.
type: application

# This is the chart version. This version number should be incremented each time you make changes
# to the chart and its templates, including the app version.
# Versions are expected to follow Semantic Versioning (https://semver.org/)
version: 0.1.0

# This is the version number of the application being deployed. This version number should be
# incremented each time you make changes to the application. Versions are not expected to
# follow Semantic Versioning. They should reflect the version the application is using.
# It is recommended to use it with quotes.
appVersion: "1.0.0"

dependencies:
  - name: redis
    version: 17.9.x 
    repository: https://charts.bitnami.com/bitnami
    condition: redis.enabled
```


**para desplegar utilizaremos los siguientes valores por defecto**

```json
# Default values for app.
# This is a YAML-formatted file.
# Declare variables to be passed into your templates.
replicaCount: 1

image:
  name: ealopezor/app:v1.0.0
  # pullPolicy: IfNotPresent
  # Overrides the image tag whose default is the chart appVersion.
  tag: "1.0"

# imagePullSecrets: []
# nameOverride: ""
# fullnameOverride: ""

# serviceAccount:
#   Specifies whether a service account should be created
#   create: true
#   Annotations to add to the service account
#   annotations: {}
#   The name of the service account to use.
#   If not set and create is true, a name is generated using the fullname template
#   name: ""

# podAnnotations: {}

# podSecurityContext: {}
#   # fsGroup: 2000

# securityContext: {}
#   # capabilities:
#   #   drop:
#   #   - ALL
#   # readOnlyRootFilesystem: true
#   # runAsNonRoot: true
#   # runAsUser: 1000

# service:
#   type: ClusterIP
#   port: 80

ingress:
  enabled: true
  # className: ""
  annotations: {}
  kubernetes.io/ingress.class: nginx
    # kubernetes.io/tls-acme: "true"
  hosts:
    - host: flask-helm.keepcoding.local
      paths:
        - path: "/"
  #         pathType: ImplementationSpecific
  # tls: []
  # #  - secretName: chart-example-tls
  # #    hosts:
  # #      - chart-example.local

resources:
  limits:
    cpu: 200m
    memory: 256Mi
  requests:
    cpu: 100m
    memory: 128Mi

autoscaling:
  enabled: false
  minReplicas: 1
  maxReplicas: 10
  targetCPUUtilizationPercentage: 80
  # targetMemoryUtilizationPercentage: 80

# nodeSelector: {}

# tolerations: []

# affinity: {}
redis:
  cluster:
    enabled: false
  metrics:
    enabled: true
```

Para lanzar desde la carpeta app utilizamos el comando

```console
helm install
```

lo cual desplegara nuestra aplicacion


realizado por Eduardo Hernandez Lopez Devops7

ealoezor@gmail.com