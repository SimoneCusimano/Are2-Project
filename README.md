# Leveraging Cognitive Computation Text Extraction Tools to Detect Gender from photos

## Introduction
The purpose of this Application is to collect a set of images (photos) and extract from them a series of
metadata with the aim of making the photos indexable and searchable.
In particular, we implemented a multi-class classification system that can detect whether a given image contains:
one or more male, one or more female, both male and female, no person.
We choose to retrieve the set of photos from Facebook, in particular we took the public photos from the User profiles.
The extraction of the metadata is implemented through the use of the Cognitive Services of Microsoft.

## Server Infrastructure
The Server on which we released the Web Application is a Linux Server. More precisely, CentOS 7 Core
x64 equipped with an HTTP Apache Server, running on port 80.
In addition, an instance of HDFS (cluster mode) runs on the server.
For safety reasons, the HDFS services are not exposed on the Internet, but they are accessible only from
the local machine. This is why we use a reverse Proxy (Kestrel), configured to reply at the location:
http://192.167.155.71/scusimano

## Application Infrastructure
To develop the project we used .NET Core because it allows to build robust and cross-platform solutions.
The following two sub-paragraphs will explain in detail the integration with Facebook services and with
the Cognitive Services of Microsoft.

### Facebook autentication
We used the Facebook Graph API, needed to access and interact with Facebook contents.
To use them, we first needed to ask for the API KEY, specifying the needed “security scopes” for the
Application.
In particular, we request “user_photos” to gain the url of the User’s public photos.
This step wasn’t immediate because Facebook requires a screencast of the Application to show explicitly
the User’s actions for performing the login, accepting to give access to the public photos, and once
accepted, Facebook redirects to our application.
After that, the request is submitted and approved/rejected by the Facebook Team.
After clicking on the “Login” button, as we said, the user is redirected to Facebook, he grants the
permissions, and is redirected back to our Homepage.
Now it is possible to obtain the list of the public images of the user, which we will forward to the
Cognitive Services of Microsoft.

## Microsoft Cognitive Services
We used the Computer Vision API (v1.0). It provides state-of-the-art algorithms to process images and return information.
For example, it can be used to determine if an image contains mature content, or it can be used to find all the faces in
an image, or to categorize the content of images.
This operation extracts a rich set of visual features based on the image content.
Two input methods are supported:
- Uploading an image
- Specifying an image URL

Within the request, there is an optional parameter to allow to choose which features to return. By
default, image categories are returned in the response. A successful response will be returned in JSON.
If the request fails, the response will contain an error code and a message to help understand what went
wrong.
For every image we call the Computer Vision API to gain some informative content, through the following call:
https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/analyze/visualFeatures=Categories,Description,Color,Faces,ImageType,Adult

The metadata we collect are:
- Categories - categorizes image content according to a taxonomy defined in documentation.
- Tags - tags the image with a detailed list of words related to the image content.
- Description - describes the image content with a complete English sentence.
- Faces - detects if faces are present. If present, generate coordinates, gender and age.
- ImageType - detects if image is clipart or a line drawing.
- Color - determines the accent color, dominan2t color, and whether an image is black&white.
- Adult - detects if the image is pornographic in nature (depicts nudity or a sex act). Sexually suggestive content is also detected.

After retrieving these informations from each photo, we show in the Homepage the miniatures of the
photos. To avoid that the user spends too much time waiting, the page is increasingly populated with
groups of ten photos.
By clicking on one photo the user can see the metadata associated.
In background we created a Task to create a JSON file containing the url of each photo and the
associated metadata. This file is stored on HDFS.
To do that, we used the REST API of WebHDFS through the following request:
http://master:50070/webhdfs/v1/[PATH]/[USER_FACEBOOK_ID]_[GUID].json?op=CREATE&overwrite=tru
e
where:
• PATH represents the location on which we want to save the file
• USER_FACEBOOK_ID represents the user associated to the file (we have a single file for each
user)
• GUID is a univocal identifier

## Deploy Steps
- Compile ASP.NET Core Application so that it could run on the server, copy it on
/home/simone.cusimano/centos7.x64 and give it appropriate permissions
dotnet publish -c release -r centos.7-x64
chmod 775 -R /home/simone.cusimano/centos7.x64

- Create an HTTPD configuration file in "/etc/httpd/conf.d/simonecusimano.conf" as root user
LimitRequestFieldSize 50000
ProxyTimeout 1200
<VirtualHost *:80>
ProxyPreserveHost On
ProxyPass /scusimano http://127.0.0.1:5001 timeout=1200
ProxyPassReverse /scusimano http://127.0.0.1:5001
ErrorLog /var/log/httpd/scusimano-error.log
CustomLog /var/log/httpd/scusimano-access.log common
</VirtualHost>

- Create a service file in "/etc/systemd/system/simonecusimano.service" as root user and give it
appropriate permission (chmod 775 /etc/systemd/system/simonecusimano.service)
[Unit]
Description=Are2Project
[Service]
ExecStart=/usr/local/bin/dotnet /home/simone.cusimano/centos7.x64/Are2Project.dll
Restart=always
RestartSec=10
SyslogIdentifier=Are2Project
User=root
WorkingDirectory=/home/simone.cusimano/centos7.x64/
Environment=ASPNETCORE_ENVIRONMENT=Production
[Install]
WantedBy=multi-user.target

- Apache Check Tests
service httpd configtest

- Service Check Tests
systemctl start simonecusimano.service
systemctl status simonecusimano.service
