provider "google" {
  project = "proyecto-final-devops"
  region  = "europe-west3"
}


terraform {
  backend "gcs" {
    bucket = "dt-tf-state-staging"
    prefix = "terraform/state"

  }
  required_providers {
    google = {
      source  = "hashicorp/google"
      version = "4.68.0"
    }
  }

  required_version = ">= 0.14"
}
