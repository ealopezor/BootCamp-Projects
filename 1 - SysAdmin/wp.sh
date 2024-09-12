#Instalacion de paquetes
sudo apt-get update
sudo apt-get install -y lvm2 nginx mariadb-server mariadb-common php-fpm php-mysql expect php-curl php-gd php-intl php-mbstring php-soap php-xml php-xmlrpc php-zip apt-transport-https

#Punto de Montaje BBDD

sudo cp -a /var/lib/mysql .
sudo parted /dev/sdc --script mklabel gpt
sudo parted -a optimal /dev/sdc --script mkpart primary ext4 2048s 100%
sudo pvcreate /dev/sdc1
sudo vgcreate vg_mysql /dev/sdc1
sudo lvcreate -n lv_mysql -l +100%FREE vg_mysql
sudo mkfs.ext4 /dev/vg_mysql/lv_mysql
sudo echo "/dev/mapper/vg_mysql-lv_mysql /var/lib/mysql ext4 defaults 0 0" >> /etc/fstab
sudo mount -a
sudo cp -a mysql/. /var/lib/mysql
sudo rm -r mysql/ 


# Configuracion NGINX
sudo cp /vagrant/nginxwp /etc/nginx/sites-available/wordpress
sudo chmod 644 /etc/nginx/sites-available/wordpress
sudo ln -s /etc/nginx/sites-available/wordpress /etc/nginx/sites-enabled/wordpress
sudo rm /etc/nginx/sites-enabled/default
sudo systemctl restart nginx

# Creacion y securizacion Base de Datos

sudo mysql --user=root < /vagrant/bbdd.sql


#Instalacion y configuracion Wordpress 


wget -c http://wordpress.org/latest.tar.gz 
tar -xzvf latest.tar.gz -C /var/www
sudo chown -R www-data:www-data /var/www/wordpress/
sudo cp /var/www/wordpress/wp-config-sample.php /var/www/wordpress/wp-config.php

sudo -u www-data sed -i 's/database_name_here/wordpress/' /var/www/wordpress/wp-config.php
sudo -u www-data sed -i 's/username_here/wpuser/' /var/www/wordpress/wp-config.php
sudo -u www-data sed -i 's/password_here/keepcoding/' /var/www/wordpress/wp-config.php

#Instalacion Filebeat

wget -qO - https://artifacts.elastic.co/GPG-KEY-elasticsearch | sudo apt-key add -
echo "deb https://artifacts.elastic.co/packages/7.x/apt stable main" | sudo tee -a /etc/apt/sources.list.d/elastic-7.x.list
sudo apt-get update
sudo apt-get install filebeat
sudo filebeat modules enable system
sudo filebeat modules enable nginx

sudo cp filebeat.yml /etc/filebeat/
sudo chown root:root /etc/filebeat/filebeat.yml
sudo chmod 600 /etc/filebeat/filebeat.yml

systemctl enable filebeat --now
