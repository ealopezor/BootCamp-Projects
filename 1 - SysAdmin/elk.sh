#Actualizacion e instalacion de paquetes basicos
wget -qO - https://artifacts.elastic.co/GPG-KEY-elasticsearch | sudo apt-key add -
echo "deb https://artifacts.elastic.co/packages/7.x/apt stable main" | sudo tee -a /etc/apt/sources.list.d/elastic-7.x.list
sudo apt-get update
sudo apt-get install -y default-jre logstash elasticsearch kibana nginx


#Punto de Montaje BBDD ELK

sudo parted -s -a optimal /dev/sdc mklabel gpt
sudo parted -s -a optimal /dev/sdc mkpart primary 0% 100%
sudo parted -s  /dev/sdc align-check optimal 1
sudo pvcreate /dev/sdc1
sudo vgcreate elk10 /dev/sdc1
sudo lvcreate -n elasticsearch -l 100%FREE elk10
sudo mkfs.ext4 /dev/mapper/elk10-elasticsearch
echo "/dev/mapper/elk10-elasticsearch /var/lib/elasticsearch/ ext4 defaults 0 0" | sudo tee -a /etc/fstab
sudo mount -a 
sudo chown -R elasticsearch:elasticsearch /var/lib/elasticsearch/
sudo chmod -R 775 /var/lib/elasticsearch/

systemctl enable elasticsearch --now
sudo systemctl start elasticsearch

#Logstash config

sudo cp -f /vagrant/02-beats-input.conf /etc/logstash/conf.d/02-beats-input.conf
sudo cp -f /vagrant/10-syslog-filter.conf /etc/logstash/conf.d/10-syslog-filter.conf
sudo cp -f /vagrant/30-elasticsearch-output.conf /etc/logstash/conf.d/30-elasticsearch-output.conf
sudo chown elasticsearch:elasticsearch /etc/logstash/conf.d
sudo chmod 644 /etc/logstash/conf.d/*.conf
sudo systemctl enable logstash --now
sudo systemctl start logstash

#Kibana Config

sudo cp /vagrant/kibaconfig /etc/nginx/sites-enabled/default
sudo chown -R elasticsearch:elasticsearch /etc/nginx/sites-enabled/default
sudo chmod -R 775 /etc/nginx/sites-enabled/default

echo "kibanaadmin:$(openssl passwd -apr1 -in /vagrant/.kibana)" | sudo tee -a /etc/nginx/htpasswd.users

sudo systemctl enable kibana --now
sudo systemctl start kibana
sudo systemctl restart nginx