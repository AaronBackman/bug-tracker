import './App.css';
import React from 'react';
import Cookies from 'universal-cookie';
import {BrowserRouter as Router, Switch, Route, Redirect} from 'react-router-dom';
import axios from 'axios';

import Home from './Home.js';
import Login from './Login';
import ProjectList from './ProjectList';
import SideBar from './SideBar';
import NavBar from './NavBar';
import ProjectPage from './ProjectPage';
import DashBoard from './DashBoard';

class App extends React.Component {
  state = {
    userNickname: ''
  }

  constructor(props) {
    super(props);

    this.setUserNickname = this.setUserNickname.bind(this);
  }

  setUserNickname(nickname) {
    this.setState({userNickname: nickname});
  }

  componentDidMount() {
    const cookies = new Cookies();
    const isAuth = !!cookies.get("email");

    // authenticated, but no user info, get info from the server
    if (isAuth && !this.state.userNickname) {
      axios.get('https://localhost:5000/api/user/' + cookies.get('email'))
      .then(response => {
        console.log(response.data);
        this.setState({userNickname: response.data.nickname});
      })
      .catch(error => {
          console.log(error);
      })
    }
  }

  render() {
    const cookies = new Cookies();
    const isAuth = !!cookies.get("email");

    return (
      <Router>
        <div className="window-flex-outer">
          {isAuth && <NavBar userNickname={this.state.userNickname} setUserNickname={this.setUserNickname} />}
          <div className="window-flex-inner">
            {isAuth && <SideBar />}

            <Redirect exact from="/" to="home" />
            {isAuth && <Redirect exact from="/login" to="/home" />}
            {!isAuth && <Redirect from="*" to="/login" />}
            <Switch>
              <Route exact path="/login" render={(props) => <Login setUserNickname={this.setUserNickname} {...props} />} />
              <Route exact path="/home" render={(props) => <Home userNickname={this.state.userNickname} {...props} />} />
              <Route exact path="/projects" render={(props) => <ProjectList userNickname={this.state.userNickname} {...props} />} />
              <Route exact path="/projects/:guid" render={(props) => <ProjectPage {...props} />} />
              <Route exact path="/dashboard" render={(props) => <DashBoard {...props} />} />
            </Switch>
          </div>
        </div>
      </Router>
    );
  }
}

export default App;
