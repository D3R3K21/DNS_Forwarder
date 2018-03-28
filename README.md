# DNS_Forwarder
1) Search for "Troubleshoot Network"
2) Click on "Change Adapter Options"
3) Right-Click and open properties on any ethernet adapters and edit the the IPV4 setting to use the following DNS server addresses
  127.0.0.2
  10.0.32.246
 4) Run the service as admin
 5) Type 'r' and enter, then enter the name of the release environment you would like to point your local service to. 
 If the solution is already open you will need to close it and reopen it for the CONSUL_SERVER setting to be updated, as it is cached when VS opens
