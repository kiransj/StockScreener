#!/bin/bash 

for i in `seq 4 4` ;
do
    #download stock prices
    filename=$(printf "cm%02dDEC2017bhav.csv.zip" $i)
    url="https://www.nseindia.com/content/historical/EQUITIES/2017/DEC/$filename"
    echo "Downloading Stock data from $url"
    curl $url -O

    SIZE=$(stat -c%s $filename)
    if [ $SIZE -gt 1000 ] 
    then
        unzip $filename
    fi
    rm $filename

    #download deliverables information
    filename=$(printf "MTO_%02d122017.DAT" $i)
    url="https://www.nseindia.com/archives/equities/mto/$filename"
    echo "Downloading Deliverables information from $url"
    curl $url -O

    SIZE=$(stat -c%s $filename)
    if [ $SIZE -lt 1000 ] 
    then
        echo "Deleteing file $filename"
        rm $filename
    else
       sed -i 1,3d $filename 
    fi

    #download circuit hitter 
    filename=$(printf "PR%02d1217.zip" $i)
    circuit_hitter=$(printf "bh%02d1217.csv" $i)
    url="https://www.nseindia.com/archives/equities/bhavcopy/pr/$filename"
    echo "Downloading Stock data from $url"
    curl $url -O

    SIZE=$(stat -c%s $filename)
    if [ $SIZE -gt 1000 ]
    then
        unzip -p $filename $circuit_hitter > $circuit_hitter
    fi
    rm $filename


done
