import React from 'react';
import { NavLink, Route, Switch } from 'react-router-dom';

import Home from '../Home/Home';
import Products from '../Products/Products';

import './App.scss'

const App = () => {
    return (
        <div>
            <header className='navbar navbar-expand navbar-dark bg-dark'>
                <div className='container'>
                    <a className='navbar-brand' href='#'>Vegan</a>
                    <button className='navbar-toggler' type='button' data-toggle='collapse' data-target='#headerNavigation'>
                        <span className='navbar-toggler-icon'></span>
                    </button>
                    <div className='collapse navbar-collapse' id='headerNavigation'>
                        <div className='navbar-nav'>
                            <NavLink className='nav-item nav-link' activeClassName='active' to='/' exact>Home</NavLink>
                            <NavLink className='nav-item nav-link' activeClassName='active' to='/producten'>Producten</NavLink>
                        </div>
                    </div>
                </div>
            </header>
            <div className='container'>
                <div className='row mt-3'>
                    <Switch>
                        <Route exact path='/' component={Home}/>
                        <Route path='/producten' component={Products}/>>
                    </Switch>
                </div>
            </div>
        </div>
    );     
};

export default App;