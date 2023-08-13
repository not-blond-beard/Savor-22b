#!/bin/bash

current_path=$(pwd)
root_path="$current_path"
while [[ ! "$root_path" =~ "Savor-22b" ]]; do
  root_path=$(dirname "$root_path")
done
root_path=${root_path%%/Savor-22b*}

project_path="$root_path/Savor-22b/backend/app"

genesis_block_path="file://$project_path/data/genesis.bin"
store_uri="rocksdb+file://$project_path/data/store?secure=false"
csv_data_resource_path="$root_path/Savor-22b/resources/savor22b/tabledata"

local_json_file="$project_path/Savor22b/appsettings.local.json"
json_file="$project_path/Savor22b/appsettings.json"

if [ ! -f $local_json_file ]; then
    cp $json_file $local_json_file
fi

jq ".GenesisBlockPath = \"$genesis_block_path\" | .StoreUri = \"$store_uri\" | .CsvDataResourcePath = \"$csv_data_resource_path\"" $local_json_file >temp.json && mv temp.json $local_json_file
