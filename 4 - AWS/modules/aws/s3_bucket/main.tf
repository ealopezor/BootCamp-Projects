resource "aws_s3_bucket" "this" {
  bucket = var.bucket
}

resource "aws_s3_bucket_website_configuration" "webconf" {
  bucket = aws_s3_bucket.this.id

  index_document {
    suffix = "index.html"
  }

  error_document {
    key = "error.html"
  }
}

resource "aws_s3_object" "index" {

  bucket       = aws_s3_bucket.this.bucket
  key          = "index.html"
  source       = "files/index.html"
  content_type = "text/html"
  etag         = filemd5("files/index.html")
  acl          = "public-read"
}

resource "aws_s3_object" "error" {

  bucket       = aws_s3_bucket.this.bucket
  key          = "error.html"
  source       = "files/error.html"
  content_type = "text/html"
  etag         = filemd5("files/error.html")
  acl          = "public-read"
}
