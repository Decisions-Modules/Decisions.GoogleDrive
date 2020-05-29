
$local:service = (get-service "servicehostmanager");

if ($local:service.status -eq "Running") {
    $local:service.Stop()
}

echo "stopping SHM..."
do { $local:service.refresh(); sleep 1; } until ($local:service.status -eq "Stopped")


echo "starting SHM..."
$local:service.Start()

do { $local:service.refresh(); sleep 1; } until ($local:service.status -eq "Running")
echo "SHM Started."