output "arn" {
  description = "S3 Bucket Name"
  value       = module.s3_website.arn
}
output "name" {
  description = "S3 Bucket Name"
  value       = module.s3_website.name
}

output "website_endpoint" {
  description = "The public url of this website."
  value       = module.s3_website.website_endpoint
}