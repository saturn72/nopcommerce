﻿To enable the service to communicate with google cloud, follow these instructions: 
https://cloud.google.com/docs/authentication/application-default-credentials.
This must be done PER SERVICE, following this specific method: https://cloud.google.com/docs/authentication/gcloud#gcloud-credentials

ONLY cash payment is supportted. install "Cash on delivery" payment module from nop commerce marketplace

settings:
- Order Settings --> Advanced --> Checkout
 - Uncheck `Terms of Service (shopping cart page)`

- Customer Settings --> Advanced --> Customer form fields
 - Uncheck `'Last name' required`
- Customer Settings --> Advanced --> Address form fields
 - Uncheck `'Zip / postal code' required`
- Customer Settings --> Advanced --> Address form fields
 - Uncheck `'Fax number' enabled`

- Vendor atribute -=> create required, single line of text attribute named "Short Description"
- Remove all UNSUPPORTED required checkout attributes