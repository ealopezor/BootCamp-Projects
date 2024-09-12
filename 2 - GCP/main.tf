terraform {
  required_providers {
    google = {
        source = "hashicorp/google"
        version = "4.49.0"
    }
  }
}

provider "google" {
  credentials = file("creden.json")

  project = "edu-keepcoding" 
  region = "europe-west3"
  zone = "europe-west3-a"
}

resource "google_compute_network" "network-vpc" {
    name = "network-vpc"
  
}

resource "google_compute_address" "ip-vm" {
  name = "static-ip"
}

resource "google_compute_instance" "terraform-vm" {
  depends_on = [
    google_compute_network.network-vpc,
    google_compute_address.ip-vm
  ]

  name = "bonus-tf-vm"
  machine_type = "e2-micro"

  boot_disk {
    initialize_params{
        image = "projects/ubuntu-os-cloud/global/images/ubuntu-2004-focal-v20230125"
    }
  }
network_interface{
    network = google_compute_network.network-vpc.name
    access_config{
        nat_ip = google_compute_address.ip-vm.address
    }
}
}

resource "google_storage_bucket" "bucket-bonus" {
  name = "bucket-bonus"
  location = "europe-west3"
}