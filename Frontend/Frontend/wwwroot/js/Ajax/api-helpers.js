function safeArray(r) {
    if (Array.isArray(r)) return r;
    if (!r || typeof r !== 'object') return [];
    if (Array.isArray(r.$values)) return r.$values;
    if (Array.isArray(r.data)) return r.data;
    if (Array.isArray(r.Data)) return r.Data;
    if (r.data && Array.isArray(r.data.$values)) return r.data.$values;
    if (r.Data && Array.isArray(r.Data.$values)) return r.Data.$values;
    return [];
}

function safeObject(r) {
    if (!r || typeof r !== 'object') return {};
    if (r.data && typeof r.data === 'object' && !Array.isArray(r.data)) return r.data;
    if (r.Data && typeof r.Data === 'object' && !Array.isArray(r.Data)) return r.Data;
    return r;
}

function ajaxGetArray(url) {
    return $.ajax({ type: 'GET', url: url, dataType: 'json', global: false })
        .then(function (r) { return safeArray(r); }, function () { return []; });
}
