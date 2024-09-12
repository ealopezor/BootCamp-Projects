module "s3_website" {
  source = "./modules/aws/s3_bucket"

  bucket = var.s3_bucket
}