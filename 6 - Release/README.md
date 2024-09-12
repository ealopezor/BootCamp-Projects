# keepcoding-devops-liberando-productos-practica-final

## Objetivo

El objetivo es mejorar un proyecto creado previamente para ponerlo en producción, a través de la adicción de una serie de mejoras.

## Proyecto inicial

El proyecto inicial es un servidor que realiza lo siguiente:

- Utiliza [FastAPI](https://fastapi.tiangolo.com/) para levantar un servidor en el puerto `8081` e implementa inicialmente dos endpoints:
  - `/`: Devuelve en formato `JSON` como respuesta `{"health": "ok"}` y un status code 200.
  - `/health`: Devuelve en formato `JSON` como respuesta `{"message":"Hello World"}` y un status code 200.

- Se han implementado tests unitarios para el servidor [FastAPI](https://fastapi.tiangolo.com/)

- Utiliza [prometheus-client](https://github.com/prometheus/client_python) para arrancar un servidor de métricas en el puerto `8000` y poder registrar métricas, siendo inicialmente las siguientes:
  - `Counter('server_requests_total', 'Total number of requests to this webserver')`: Contador que se incrementará cada vez que se haga una llamada a alguno de los endpoints implementados por el servidor (inicialmente `/` y `/health`)
  - `Counter('healthcheck_requests_total', 'Total number of requests to healthcheck')`: Contador que se incrementará cada vez que se haga una llamada al endpoint `/health`.
  - `Counter('main_requests_total', 'Total number of requests to main endpoint')`: Contador que se incrementará cada vez que se haga una llamada al endpoint `/`.

## Software necesario

Es necesario disponer del siguiente software:

- `Python` en versión `3.8.5` o superior, disponible para los diferentes sistemas operativos en la [página oficial de descargas](https://www.python.org/downloads/release/python-385/)

- `virtualenv` para poder instalar las librerías necesarias de Python, se puede instalar a través del siguiente comando:

    ```sh
    pip3 install virtualenv
    ```

    En caso de estar utilizando Linux y el comando anterior diera fallos se debe ejecutar el siguiente comando:

    ```sh
    sudo apt-get update && sudo apt-get install -y python3.8-venv
    ```

- `Docker` para poder arrancar el servidor implementado a través de un contenedor Docker, es posible descargarlo a [través de su página oficial](https://docs.docker.com/get-docker/).

## Ejecución de servidor

### Ejecución directa con Python

1. Instalación de un virtualenv, **realizarlo sólo en caso de no haberlo realizado previamente**:
   1. Obtener la versión actual de Python instalada para crear posteriormente un virtualenv:

        ```sh
        python3 --version
        ```

        El comando anterior mostrará algo como lo mostrado a continuación:ç

        ```sh
            Python 3.10.7
        ```

   2. Crear de virtualenv en la raíz del directorio para poder instalar las librerías necesarias:

       - En caso de en el comando anterior haber obtenido `Python 3.10.7`

            ```sh
            python3 -m venv venv
            ```



2. Activar el virtualenv creado en el directorio `venv` en el paso anterior:

     ```sh
     source venv/bin/activate
     ```

3. Instalar las librerías necesarias de Python, recogidas en el fichero `requirements.txt`, **sólo en caso de no haber realizado este paso previamente**. Es posible instalarlas a través del siguiente comando:

    ```sh
    pip3 install -r requirements.txt
    ```

4. Ejecución del código para arrancar el servidor:

    ```sh
    python3 src/app.py
    ```

5. La ejecución del comando anterior debería mostrar algo como lo siguiente:

    ```sh
    [2022-04-16 09:44:22 +0000] [1] [INFO] Running on http://0.0.0.0:8081 (CTRL + C to quit)
    ```

### Ejecución a través de un contenedor Docker

1. Crear una imagen Docker con el código necesario para arrancar el servidor:

    ```sh
    docker build -t practicasreehl .
    ```

2. Arrancar la imagen construida en el paso anterior mapeando los puertos utilizados por el servidor de FastAPI y el cliente de prometheus:

    ```sh
    docker run -d -p 8000:8000 -p 8081:8081 --name practicasreehl prachticasreehl
    ```

3. Obtener los logs del contenedor creado en el paso anterior:

    ```sh
    docker logs -f practicasreehl
    ```

4. La ejecución del comando anterior debería mostrar algo como lo siguiente:

    ```sh
    [2022-04-16 09:44:22 +0000] [1] [INFO] Running on http://0.0.0.0:8081 (CTRL + C to quit)
    ```

## Comprobación de endpoints de servidor y métricas

Una vez arrancado el servidor, utilizando cualquier de las formas expuestas en los apartados anteriores, es posible probar las funcionalidades implementadas por el servidor:

- Comprobación de servidor FastAPI, a través de llamadas a los diferentes endpoints:

  - Realizar una petición al endpoint `/`

      ```sh
      curl -X 'GET' \
      'http://0.0.0.0:8081/' \
      -H 'accept: application/json'
      ```

      Debería devolver la siguiente respuesta:

      ```json
      {"message":"Hello World"}
      ```

  - Realizar una petición al endpoint `/health`

      ```sh
      curl -X 'GET' \
      'http://0.0.0.0:8081/health' \
      -H 'accept: application/json' -v
      ```

      Debería devolver la siguiente respuesta.

      ```json
      {"health": "ok"}
      ```
- Realizar una petición al endpoint `/bye`

      ```sh
      curl -X 'GET' \
      'http://0.0.0.0:8081/bye' \
      -H 'accept: application/json' -v
      ```

      Debería devolver la siguiente respuesta.

      ```json
      {"message": "Bye, Bye, Bye..."}
      ```

## Tests

Se ha implementado tests unitarios para probar el servidor FastAPI, estos están disponibles en el archivo `src/tests/app_test.py`.

Es posible ejecutar los tests de diferentes formas:

- Ejecución de todos los tests:

    ```sh
    pytest
    ```

- Ejecución de todos los tests y mostrar cobertura:

    ```sh
    pytest --cov
    ```

- Ejecución de todos los tests y generación de report de cobertura:

    ```sh
    pytest --cov --cov-report=html
    ```

## Monitoring

# Prometheus

- para ver las metricas utilizaremos un prometheus que utiliza un cluster de Kubernetes

```console
    minikube start --kubernetes-version='v1.21.1' \
 --memory=4096 \
 --addons="metrics-server,default-storageclass,storage-provisioner" \
 -p monitoring-test
```
- Añadir repo de helm

   ```console
    helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
   ```
- Despliegue del chart de Prometheus

 ```console
  helm -n monitoring upgrade --install prometheus prometheus-community/kube-prometheus-stack -f ./helm/kube-prometheus-stack/values.yaml --create-namespace --wait
 ```

-  Portforward al service de Prometheus
```console
kubectl -n monitoring port-forward svc/prometheus-kube-prometheus-prometheus 9090:9090
```
- Portforward al service de grafana

```console
kubectl -n monitoring port-forward svc/prometheus-grafana 3000:3000
```

- Habilitar Metrics server

  ```console
  minikube addons enable metrics-server -p monitoring-tests
  ```

- Desplegando la aplicacion con Helm

```console
helm -n fast-api upgrade my-app --wait --install --create-namespace fast-api-webapp
```

- Acceder a la dirección `http://localhost:3000` en el navegador para acceder a Grafana, las credenciales por defecto son `admin` para el usuario y `prom-operator` para la contraseña.

- Acceder a la dirección `http://localhost:9090` para acceder al Prometheus, **por defecto no se necesita autenticación**.

- Empezar a realizar diferentes peticiones al servidor de fastapi, es posible ver los endpoints disponibles y realizar peticiones a los mismos a través de la URL `http://localhost:8081/docs` utilizando swagger

- Acceder al dashboard creado para observar las peticiones al servidor a través de la URL `http://localhost:3000/dashboards`, seleccionando una vez en ella la opción Import y en el siguiente paso seleccionar **Upload JSON File** y seleccionar el archivo presente en esta carpeta llamado `practica_edu.json`.

## Alertas de alto consumo de CPU

Se ha configurado en el chart de kube-prometheus-stack/values los valores para enviar alertas a un webhook en un canl de slack

```python
receivers:
    - name: 'null'
    - name: 'slack'
      slack_configs:
      - api_url: 'https://hooks.slack.com/services/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' # <--- AÑADIR EN ESTA LÍNEA EL WEBHOOK CREADO
        send_resolved: true
        channel: '#alarms-notifications-keepcoding-devops-7' # <--- AÑADIR EN ESTA LÍNEA EL CANAL
        title: '[{{ .Status | toUpper }}{{ if eq .Status "firing" }}:{{ .Alerts.Firing | len }}{{ end }}] Monitoring Event Notification'
        text: |-
          {{ range .Alerts }}
            *Alert:* {{ .Labels.alertname }} - `{{ .Labels.severity }}`
            *Description:* {{ .Annotations.message }}
            *Graph:* <{{ .GeneratorURL }}|:chart_with_upwards_trend:> *Runbook:* <{{ .Annotations.runbook_url }}|:spiral_note_pad:>
            *Details:*
            {{ range .Labels.SortedPairs }} • *{{ .Name }}:* `{{ .Value }}`
            {{ end }}
          {{ end }}

```

- Obtener el pod creado en el paso 1 para poder lanzar posteriormente un comando de prueba de extres, así como seleccionarlo en el menú desplegable del panel de grafana:

   ```sh
   export POD_NAME=$(kubectl get pods --namespace fast-api -l "app.kubernetes.io/name=fast-api-webapp,app.kubernetes.io/instance=my-app" -o jsonpath="{.items[0].metadata.name}")
   echo $POD_NAME
   ```

   Se debería obtener un resultado similar al siguiente:

   ```sh
   my-app-fast-api-webapp-585bf945cc-lvvpv
   ```

- Utilizar el resultado obtenido en el paso anterior para seleccionar en el dashboard creado de grafana para seleccionar el pod del que obtener información, seleccionando este a través del menú desplegable de nombre `pod`.

- Acceder mediante una shell interactiva al contenedor `fast-api-webapp` del pod obtenido en el paso anterior:

    ```sh
    kubectl -n fast-api exec --stdin --tty $POD_NAME -c fast-api-webapp -- /bin/sh
    ```

- Dentro de la shell en la que se ha accedido en el paso anterior instalar y utilizar los siguientes comandos para descargar un proyecto de github que realizará pruebas de extress:

    - Instalar los binarios necesarios en el pod

        ```sh
        apk update && apk add git go
        ```

    - Descargar el repositorio de github y acceder a la carpeta de este, donde se realizará la compilación del proyecot:

        ```sh
        git clone https://github.com/jaeg/NodeWrecker.git
        cd NodeWrecker
        go build -o extress main.go
        ```

    - Ejecución del binario obtenido de la compilación del paso anterior que realizará una prueba de extress dentro del pod:

        ```sh
        ./extress -abuse-memory -escalate -max-duration 10000000
        ```

- resultados en las imagenes, se pueden observar las alertas en el slack, y los accesos a los endpoints en grafana.


## Pipelines de CI/CD

He utilizado Github actions para definir los pipelines de la practica, pueden verse en la carpeta .github/workflows 

