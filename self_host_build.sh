#fetch latest source version
sudo git pull
#stop exist nopCommerce instance
sudo sudo systemctl stop nopCommerce
# build nopCommerce
sudo dotnet build ./src/Presentation/Nop.Web/Nop.Web.csproj --configuration Release --output ../../nopCommerce
#build KM.Api plugin
sudo dotnet build ./src/Plugins/KM.Api/KM.Api.csproj --configuration Release 
#copy plugin to current store
sudo cp -r ./src/Presentation/Nop.Web/Plugins/KM.Api ../../nopCommerce/Plugins

#build Cash on delivery plugin
sudo dotnet build ./src/Plugins/Nop.Plugin.Payments.CashOnDelivery/Nop.Plugin.Payments.CashOnDelivery.csproj --configuration Release 
#copy plugin to current store
sudo cp -r ./src/Presentation/Nop.Web/Plugins/Payments.CashOnDelivery ../../nopCommerce/Plugins

#start nopCommerce instance
cd ../../nopCommerce
sudo dotnet Nop.Web.dll
#restart nginx
sudo sudo systemctl restart nginx
