import React, { useEffect, useState } from 'react';

const Products = () => {
    const [products, setProducts] = useState([]);
    const [page, setPage] = useState(1);

    useEffect(() => {
        fetch('https://localhost:5001/api/products?page=' + page)
        .then(res => res.json())
        .then(data => setProducts(data.items))
    }, [page]);

    return (
        <div className='row row-cols-4'>
        { products.map(product => (
            <div class="col mb-3">
                <div key={product.id} className='card'>
                    <div className='card-body'>{product.name}</div>
                </div>
            </div>
        )) }
        </div>
    );
}

export default Products;