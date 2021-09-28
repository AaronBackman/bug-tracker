import React from "react";
import Cookies from "universal-cookie";

const axios = require("axios");

class Login extends React.Component {
  state = {
    email: "",
  };

  constructor(props) {
    super(props);

    this.handleClick = this.handleClick.bind(this);
  }

  handleClick(e) {
    e.preventDefault();

    const cookies = new Cookies();

    axios
      .get("https://localhost:5000/api/user/" + this.state.email)
      .then((response) => {
        console.log(response.data);

        if (response.data) {
          cookies.set("email", response.data.email, {
            path: "/",
            sameSite: "strict",
          });
          this.props.setUserNickname(response.data.nickname);
        } else {
          console.log("User not found");
        }
      })
      .catch((error) => {
        console.log(error);
      });
  }

  render() {
    return (
      <div>
        <form>
          <input
            type="text"
            value={this.state.email}
            onChange={(e) => this.setState({ email: e.target.value })}
          />
        </form>
        <div onClick={this.handleClick}>ready</div>
      </div>
    );
  }
}

export default Login;
