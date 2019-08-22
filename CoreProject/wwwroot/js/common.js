function getSubDomain() {
    var getHostname = window.location.hostname;
    //console.log(getHostname);
    var domainNameList = getHostname.split('.');
    var subDomainName = domainNameList[0];
    if (subDomainName == 'localhost') {
        subDomainName = 'cart4all';
    }
    //console.log(subDomainName);
    return subDomainName;
}